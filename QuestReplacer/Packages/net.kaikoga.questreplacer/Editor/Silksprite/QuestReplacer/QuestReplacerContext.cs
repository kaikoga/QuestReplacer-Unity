using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerContext
    {
        readonly Transform[] _targets;
        readonly QuestTypeFilter[] _componentFilters;
        readonly QuestReplacement[] _replacements;

        readonly Dictionary<Type, SerializedProperty[]> _cachedTargetProperties = new Dictionary<Type, SerializedProperty[]>();

        QuestStatus? _materialQuestStatus;
        QuestStatus? _meshQuestStatus;
        public QuestStatus QuestMaterialStatus => _materialQuestStatus ?? (_materialQuestStatus = ToQuestStatus<Material>()).Value;
        public QuestStatus QuestMeshStatus => _meshQuestStatus ?? (_meshQuestStatus = ToQuestStatus<Mesh>()).Value;


        public QuestReplacerContext(IEnumerable<Transform> targets, IEnumerable<QuestTypeFilter> componentFilters, IEnumerable<QuestReplacement> pairs)
        {
            _targets = targets.Where(target => target).ToArray();
            _componentFilters = (componentFilters != null) ? componentFilters.Reverse().ToArray() : DeepCollectComponentTypes().ToTypeFilters().Reverse().ToArray();
            _replacements = pairs.ToArray();
        }

        public QuestStatus ToQuestStatus<T>()
            where T : Object
        {
            return DeepCollectReferences<T>()
                .SelectMany(c => _replacements.Select(r => r.GetStatus(c)).Where(s => s != QuestStatus.Unmanaged))
                .Aggregate(QuestStatus.Either, (accumulator, v) => accumulator.Merge(v));
        }

        public IEnumerable<Type> DeepCollectComponentTypes()
        {
            return DeepCollectComponents(true).Select(component => component.GetType()).Distinct().OrderBy(t => t.FullName);
        }

        public IEnumerable<T> DeepCollectReferences<T>()
            where T : Object
        {
            return DeepCollectProperties<T>().Select(prop => (T)prop.objectReferenceValue);
        }

        public void DeepOverrideReferences<T>(bool toRight)
            where T : Object
        {
            var serializedObjects = new HashSet<SerializedObject>(); 
            foreach (var prop in DeepCollectProperties<T>())
            {
                // T might be Object, so do all replacements but ignore unregistered components
                var toValue = _replacements.Query(prop.objectReferenceValue, toRight);
                if (!toValue) continue;
                prop.objectReferenceValue = toValue;
                serializedObjects.Add(prop.serializedObject);
            }

            foreach (var serializedObject in serializedObjects)
            {
                Undo.RecordObject(serializedObject.targetObject, "Quest Replacer");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        IEnumerable<Component> DeepCollectComponents(bool ignoreFilters) => _targets.SelectMany(target =>
        {
            return target.GetComponentsInChildren<Component>(true)
                .Where(component =>
                    {
                        return component // Missing Script
                               && (ignoreFilters || _componentFilters.Any(componentFilter => componentFilter.Includes(component.GetType()))); // exclude QuestReplacer itself (and else)
                    });
        });

        IEnumerable<SerializedProperty> DeepCollectProperties<T>()
            where T : Object
        {
            if (_cachedTargetProperties.TryGetValue(typeof(T), out var properties)) return properties;

            return _cachedTargetProperties[typeof(T)] = DeepCollectComponents(false).SelectMany(CollectProperties<T>).ToArray();
        }

        static IEnumerable<SerializedProperty> CollectProperties<T>(Component component)
            where T : Object
        {
            var serializedObj = new SerializedObject(component);
            var it = serializedObj.GetIterator();
            while (it.Next(true))
            {
                if (it.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (it.objectReferenceValue is T)
                {
                    yield return it.Copy();
                }
            }
        }
    }
}