using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    [CustomPropertyDrawer(typeof(QuestReplacerConfig))]
    public class QuestReplacerConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
        {
            var platform = serializedProperty.FindPropertyRelative(nameof(QuestReplacerConfig.platform));
            var generateMode = serializedProperty.FindPropertyRelative(nameof(QuestReplacerConfig.generateMode));
            var manageMaterials = serializedProperty.FindPropertyRelative(nameof(QuestReplacerConfig.manageMaterials));
            var manageMeshes = serializedProperty.FindPropertyRelative(nameof(QuestReplacerConfig.manageMeshes));
            
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, platform);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, generateMode);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, manageMaterials);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, manageMeshes);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        }
    }
}