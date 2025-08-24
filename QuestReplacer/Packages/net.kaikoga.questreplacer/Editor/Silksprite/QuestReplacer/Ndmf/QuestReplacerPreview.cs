#if QUESTREPLACER_NDMF_SUPPORT

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using nadena.dev.ndmf.runtime;
using Silksprite.QuestReplacer.Context;
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
                .GroupBy(r => context.GetAvatarRootCrossPlatform(r.gameObject)?.transform)
                .SelectMany(g =>
                {
                    var coordinator = QuestReplacerCoordinatorFactory.FromAvatarRoot(g.Key);
                    if (!QuestReplacerPlatformDetector.TryGetPlatformForAvatar(g.Key, out var replacerPlatform))
                    {
                        replacerPlatform = QuestReplacerBuildPlatform.Generic;
                    }
                    return g.Select(renderer => RenderGroup.For(renderer).WithData((coordinator, replacerPlatform)));
                })
                .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var (coordinator, platform) = group.GetData<(QuestReplacerCoordinator, QuestReplacerBuildPlatform)>();
            return new QuestReplacedNode(coordinator, platform).Initialize(proxyPairs);
        }
        
        class QuestReplacedNode : IRenderFilterNode
        {
            readonly QuestReplacerCoordinator _coordinator;
            readonly QuestReplacerBuildPlatform _platform;
            readonly Dictionary<Material, Material> _tmpMaterials = new();

            public RenderAspects WhatChanged => RenderAspects.Material;

            public QuestReplacedNode(QuestReplacerCoordinator coordinator, QuestReplacerBuildPlatform platform)
            {
                _coordinator = coordinator;
                _platform = platform;
            }

            public Task<IRenderFilterNode> Initialize(IEnumerable<(Renderer, Renderer)> proxyPairs)
            {
                foreach (var (original, _) in proxyPairs)
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

            public void OnFrame(Renderer original, Renderer proxy)
            {
                proxy.sharedMaterials = original.sharedMaterials
                    .Select(material => material ? _tmpMaterials.GetValueOrDefault(material, material) : null)
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

    static class ContextExtension
    {
        // Cross platform nadena.dev.ndmf.preview.ComputeContextQueries.GetAvatarRoot
        public static GameObject GetAvatarRootCrossPlatform(this ComputeContext context, GameObject obj)
        {
            if (obj == null) return null;

            var avatarRoot = RuntimeUtil.FindAvatarInParents(obj.transform);
            GameObject candidate = null;
            foreach (var elem in context.ObservePath(obj.transform))
            {
                candidate = elem.gameObject;
                if (elem == avatarRoot)
                {
                    break;
                }
            }
            return candidate;
        }
    }
}

#endif