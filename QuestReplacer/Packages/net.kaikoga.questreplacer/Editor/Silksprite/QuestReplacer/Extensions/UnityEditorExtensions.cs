using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class UnityEditorExtensions
    {
        public static QuestStatus ToQuestStatus<T>(this Transform transform, IEnumerable<QuestReplacement> replacements)
        where T : Object
        {
            return transform.DeepCollectReferences<T>()
                .SelectMany(c => replacements.Select(r => r.GetStatus(c)).Where(s => s != QuestStatus.Unmanaged))
                .Aggregate(QuestStatus.Either, (accumulator, v) => accumulator.Merge(v));
        }
        
        public static IEnumerable<T> DeepCollectReferences<T>(this Transform transform)
            where T : Object
        {
            return DeepCollectProperties<T>(transform).Select(prop => (T)prop.objectReferenceValue);
        }
        
        public static void DeepOverrideReferences<T>(this Transform transform, IEnumerable<QuestReplacement> replacements, bool toRight)
            where T : Object
        {
            var questReplacements = replacements.ToArray();
            foreach (var prop in DeepCollectProperties<T>(transform))
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
        
        static IEnumerable<SerializedProperty> DeepCollectProperties<T>(this Transform transform)
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
                foreach (var o in child.DeepCollectProperties<T>()) yield return o;
            }
        }
    }
}