using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacementReorderableList : ReorderableList
    {
        public QuestReplacementReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
        {
            drawHeaderCallback = rect =>
            {
                var left = new Rect(rect.x, rect.y, rect.width - 50f, rect.height);
                QuestReplacerGUI.Header(left, serializedProperty.displayName);
                var right = new Rect(rect.xMax - 50f, rect.y, 50f, rect.height);
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    var newArraySize = EditorGUI.DelayedIntField(right, elements.arraySize);
                    if (changed.changed)
                    {
                        elements.arraySize = Math.Min(newArraySize, elements.arraySize + 10);
                    }
                }
            };

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, elements.GetArrayElementAtIndex(index));
            };
        }
    }
}