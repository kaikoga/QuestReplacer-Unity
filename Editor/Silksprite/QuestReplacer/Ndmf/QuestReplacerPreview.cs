using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using nadena.dev.ndmf.runtime;
using Silksprite.QuestReplacer.Platform;
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
                    var reusableCoordinator = ReusableQuestReplacerCoordinator.FromAvatarRoot(g.Key, false);
                    if (!QuestReplacerPlatformDetector.TryGetPlatformForAvatar(g.Key, out var replacerPlatform))
                    {
                        replacerPlatform = QuestReplacerBuildPlatform.Generic;
                    }
                    return g.Select(renderer => RenderGroup.For(renderer).WithData((reusableCoordinator, replacerPlatform)));
                })
                .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var (reusableCoordinator, platform) = group.GetData<(ReusableQuestReplacerCoordinator, QuestReplacerBuildPlatform)>();
            return new QuestReplacedNode(reusableCoordinator, platform).Initialize(proxyPairs);
        }

        class QuestReplacedNode : IRenderFilterNode
        {
            readonly ReusableQuestReplacerCoordinatorReference _reusableCoordinatorRef;
            readonly QuestReplacerBuildPlatform _platform;
            readonly Dictionary<Material, Material> _tmpMaterials = new();

            public RenderAspects WhatChanged => RenderAspects.Material;

            public QuestReplacedNode(ReusableQuestReplacerCoordinator reusableCoordinator, QuestReplacerBuildPlatform platform)
            {
                _reusableCoordinatorRef = reusableCoordinator.Acquire();
                _platform = platform;
            }

            public Task<IRenderFilterNode> Initialize(IEnumerable<(Renderer, Renderer)> proxyPairs)
            {
                foreach (var (original, _) in proxyPairs)
                {
                    foreach (var material in original.sharedMaterials.Where(material => material))
                    {
                        if (!_reusableCoordinatorRef.Query(material, _platform, out var toValue))
                        {
                            continue;
                        }
                        if (_tmpMaterials.TryGetValue(material, out _))
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
                _reusableCoordinatorRef.Dispose();
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