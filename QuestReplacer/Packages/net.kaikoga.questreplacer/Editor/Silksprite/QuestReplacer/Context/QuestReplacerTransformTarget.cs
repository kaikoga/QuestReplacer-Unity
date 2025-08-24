using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context
{
    public class QuestReplacerTransformTarget
    {
        readonly Transform[] _targets;
        readonly QuestTypeFilter[] _componentFilters;
        public QuestReplacerTransformTarget(Transform[] targets, QuestTypeFilter[] componentFilters)
        {
            _targets = targets;
            _componentFilters = componentFilters;
        }
        public IEnumerable<Type> DeepCollectComponentTypes()
        {
            return DeepCollectComponents(true).Select(component => component.GetType()).Distinct().OrderBy(t => t.FullName);
        }

        public IEnumerable<Component> DeepCollectComponents(bool ignoreFilters)
        {
            return _targets
                .SelectMany(target => target.GetComponentsInChildren<Component>(true))
                .Where(component =>
                {
                    return component // Missing Script
                           && (ignoreFilters || _componentFilters.Any(componentFilter => componentFilter.Includes(component.GetType()))); // exclude QuestReplacer itself (and else)
                });
        }
    }
}
