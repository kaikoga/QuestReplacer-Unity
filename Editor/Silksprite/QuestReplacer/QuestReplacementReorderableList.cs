using System;
using Silksprite.Loch;
using Silksprite.Loch.IMGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacementReorderableList : ReorderableList
    {
        public QuestReplacementReorderableList(SerializedObject serializedObject, LocalizedProperty lop) : base(serializedObject, lop.Property)
        {
            drawHeaderCallback = rect =>
            {
                var left = new Rect(rect.x, rect.y, rect.width - 50f, rect.height);
                LGUI.Header(left, lop.Loc);
                var right = new Rect(rect.xMax - 50f, rect.y, 50f, rect.height);
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    var newArraySize = EditorGUI.DelayedIntField(right, lop.Property.arraySize);
                    if (changed.changed)
                    {
                        lop.Property.arraySize = Math.Min(newArraySize, lop.Property.arraySize + 10);
                    }
                }
            };

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, lop.Property.GetArrayElementAtIndex(index));
            };
        }
    }
}