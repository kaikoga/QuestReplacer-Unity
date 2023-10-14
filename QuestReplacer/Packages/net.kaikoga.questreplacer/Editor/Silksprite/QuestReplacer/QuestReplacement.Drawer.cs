using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    [CustomPropertyDrawer(typeof(QuestReplacement))]
    public class QuestReplacementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth /= 3;
            var width = position.width / 2f - 6f;
            var left = new Rect(position.x, position.y, width, position.height); 
            var center = new Rect(position.x + width, position.y, 12f, position.height); 
            var right = new Rect(position.xMax - width, position.y, width, position.height);
            var serializedLeft = property.FindPropertyRelative("left");
            var serializedRight = property.FindPropertyRelative("right");
            EditorGUI.PropertyField(left, serializedLeft);
            GUI.Label(center, serializedLeft.objectReferenceInstanceIDValue == serializedRight.objectReferenceInstanceIDValue ? "=" : "/");
            EditorGUI.PropertyField(right, serializedRight);
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}