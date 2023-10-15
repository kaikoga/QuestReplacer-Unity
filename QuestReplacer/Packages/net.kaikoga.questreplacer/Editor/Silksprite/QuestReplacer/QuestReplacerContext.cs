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
        readonly QuestReplacement[] _replacements;

        readonly Dictionary<Type, SerializedProperty[]> _cachedTargetProperties = new Dictionary<Type, SerializedProperty[]>();

        public QuestReplacerContext(IEnumerable<Transform> targets, IEnumerable<QuestReplacement> pairs)
        {
            _targets = targets.Where(target => target).ToArray();
            _replacements = pairs.ToArray();
        }

        public QuestStatus ToQuestStatus<T>()
            where T : Object
        {
            return DeepCollectReferences<T>()
                .SelectMany(c => _replacements.Select(r => r.GetStatus(c)).Where(s => s != QuestStatus.Unmanaged))
                .Aggregate(QuestStatus.Either, (accumulator, v) => accumulator.Merge(v));
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
                if (toRight)
                {
                    var replacement = _replacements.FirstOrDefault(r => r.left == prop.objectReferenceValue);
                    if (replacement == null) continue;

                    prop.objectReferenceValue = replacement.right;
                    serializedObjects.Add(prop.serializedObject);
                }
                else
                {
                    var replacement = _replacements.FirstOrDefault(r => r.right == prop.objectReferenceValue);
                    if (replacement == null) continue;

                    prop.objectReferenceValue = replacement.left;
                    serializedObjects.Add(prop.serializedObject);
                }

            }

            foreach (var serializedObject in serializedObjects)
            {
                Undo.RecordObject(serializedObject.targetObject, "Quest Replacer");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }
        
        IEnumerable<SerializedProperty> DeepCollectProperties<T>()
            where T : Object
        {
            if (_cachedTargetProperties.TryGetValue(typeof(T), out var properties)) return properties;

            return _cachedTargetProperties[typeof(T)] = _targets.SelectMany(DeepCollectProperties<T>).ToArray();
        }

        static IEnumerable<SerializedProperty> DeepCollectProperties<T>(Transform transform)
            where T : Object
        {
            foreach (var component in transform.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue; // Missing Script
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
}