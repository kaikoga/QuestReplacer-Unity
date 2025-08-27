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
            var targetVRChatAnimations = serializedProperty.FindPropertyRelative(nameof(QuestReplacerConfig.targetVRChatAnimations));
            
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, platform);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, generateMode);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, manageMaterials);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, manageMeshes);
#if QUESTREPLACER_NDMF_SUPPORT
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, targetVRChatAnimations, new GUIContent("NDMF VRChat Animations"));
#endif
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
#if QUESTREPLACER_NDMF_SUPPORT
            const int propertyCount = 5;
#else
            const int propertyCount = 4;
#endif
            return EditorGUIUtility.singleLineHeight * propertyCount + EditorGUIUtility.standardVerticalSpacing * (propertyCount - 1);
        }
    }
}