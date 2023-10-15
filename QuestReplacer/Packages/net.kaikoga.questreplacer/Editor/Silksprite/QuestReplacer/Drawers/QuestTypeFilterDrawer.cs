using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Drawers
{
    [CustomPropertyDrawer(typeof(QuestTypeFilter))]
    public class QuestTypeFilterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 1;
            var left = new Rect(position.x, position.y, 18f, position.height); 
            var right = new Rect(position.x + 18f, position.y, position.width - 18f, position.height);
            var serializedLeft = property.FindPropertyRelative("isIncluded");
            var serializedRight = property.FindPropertyRelative("typePrefix");
            EditorGUI.PropertyField(left, serializedLeft);
            EditorGUI.PropertyField(right, serializedRight);
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}