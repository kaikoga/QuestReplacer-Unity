using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerContext
    {
        readonly Transform[] _targets;
        readonly QuestReplacement[] _replacements;

        public QuestReplacerContext(IEnumerable<Transform> targets, IEnumerable<QuestReplacement> pairs)
        {
            _targets = targets.ToArray();
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
            foreach (var prop in DeepCollectProperties<T>())
            {
                // T might be Object, so do all replacements but ignore unregistered components
                if (toRight)
                {
                    var replacement = _replacements.FirstOrDefault(r => r.left == prop.objectReferenceValue);
                    if (replacement != null)
                    {
                        prop.objectReferenceValue = replacement.right;
                    }
                }
                else
                {
                    var replacement = _replacements.FirstOrDefault(r => r.right == prop.objectReferenceValue);
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
        
        IEnumerable<SerializedProperty> DeepCollectProperties<T>() where T : Object => _targets.SelectMany(DeepCollectProperties<T>);

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