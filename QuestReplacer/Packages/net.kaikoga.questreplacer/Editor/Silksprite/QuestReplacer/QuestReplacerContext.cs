using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerContext
    {
        readonly Transform _target;
        readonly List<QuestReplacement> _replacements;

        public QuestReplacerContext(Transform target, List<QuestReplacement> pairs)
        {
            _target = target;
            _replacements = pairs;
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
            var questReplacements = _replacements.ToArray();
            foreach (var prop in DeepCollectProperties<T>())
            {
                // T might be Object, so do all replacements but ignore unregistered components
                if (toRight)
                {
                    var replacement = questReplacements.FirstOrDefault(r => r.left == prop.objectReferenceValue);
                    if (replacement != null)
                    {
                        prop.objectReferenceValue = replacement.right;
                    }
                }
                else
                {
                    var replacement = questReplacements.FirstOrDefault(r => r.right == prop.objectReferenceValue);
                    if (replacement != null)
                    {
                        prop.objectReferenceValue = replacement.left;
                    }
                }

                Undo.RecordObject(prop.serializedObject.targetObject, "Quest Replacer");
                prop.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(prop.serializedObject.targetObject);
            }
        }
        
        IEnumerable<SerializedProperty> DeepCollectProperties<T>() where T : Object => DeepCollectProperties<T>(_target);

        static IEnumerable<SerializedProperty> DeepCollectProperties<T>(Transform transform)
            where T : Object
        {
            foreach (var component in transform.GetComponents<Component>())
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

            foreach (Transform child in transform)
            {
                foreach (var o in DeepCollectProperties<T>(child)) yield return o;
            }
        }
    }
}