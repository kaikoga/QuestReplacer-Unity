#if QUEST_REPLACER_NDMF_SUPPORT

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    class QuestReplacerPreview : IRenderFilter
    {
        public bool CanEnableRenderers => false;

        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            foreach (var replacer in context.GetComponentsByType<QuestReplacer>())
            {
                context.Observe(replacer);
            }

            return context.GetComponentsByType<Renderer>()
                .Where(r => r is SkinnedMeshRenderer or MeshRenderer)
                .Where(r => context.ActiveInHierarchy(r.gameObject))
                .GroupBy(r => context.GetAvatarRoot(r.gameObject))
                .SelectMany(g =>
                {
                    var reusable = QuestReplacerReusableCoordinator.FromAvatarRoot(g.Key.transform);
                    if (!QuestReplacerPlatformDetector.TryGetPlatformForAvatar(g.Key.gameObject, out var replacerPlatform))
                    {
                        replacerPlatform = QuestReplacerBuildPlatform.Generic;
                    }
                    return g.Select(renderer => RenderGroup.For(renderer).WithData((reusable, replacerPlatform)));
                })
                .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var (reusable, platform) = group.GetData<(QuestReplacerReusableCoordinator, QuestReplacerBuildPlatform)>();
            return new QuestReplacedNode(reusable, platform).Initialize(proxyPairs, context);
        }
        
        class QuestReplacedNode : IRenderFilterNode
        {
            readonly QuestReplacerReusableCoordinator _reusable;
            readonly QuestReplacerCoordinator _coordinator;
            readonly QuestReplacerBuildPlatform _platform;
            readonly Dictionary<Material, Material> _tmpMaterials = new();

            public RenderAspects WhatChanged => RenderAspects.Material;

            public QuestReplacedNode(QuestReplacerReusableCoordinator reusable, QuestReplacerBuildPlatform platform)
            {
                _reusable = reusable;
                _coordinator = _reusable.Coordinator;
                _platform = platform;
            }

            public Task<IRenderFilterNode> Initialize(IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
            {
                foreach (var (original, proxy) in proxyPairs)
                {
                    foreach (var material in original.sharedMaterials)
                    {
                        if (!_coordinator.Query(material, _platform, out var toValue))
                        {
                            continue;
                        }
                        var toMaterial = new Material(toValue);
                        _tmpMaterials.Add(material, toMaterial);
                        ObjectRegistry.RegisterReplacedObject(material, toMaterial);
                    }
                }
                return Task.FromResult<IRenderFilterNode>(this);
            }

            public Task<IRenderFilterNode> Refresh(IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context, RenderAspects updatedAspects)
            {
                var result = _reusable.Coordinator != _coordinator ? null : this;
                return Task.FromResult<IRenderFilterNode>(result);
            }

            public void OnFrame(Renderer original, Renderer proxy)
            {
                proxy.sharedMaterials = original.sharedMaterials
                    .Select(material => _tmpMaterials.GetValueOrDefault(material, material))
                    .ToArray();
            }

            public void Dispose()
            {
                foreach (var tmpMaterial in _tmpMaterials.Values)
                {
                    Object.DestroyImmediate(tmpMaterial);
                }
            }
        }
    }
}

#endif