using Silksprite.Loch.Extensions;
using Silksprite.Loch.IMGUI;
using Silksprite.QuestReplacer.Assets;
using UnityEditor;
using UnityEngine;
using static Silksprite.Loch.Tools.LochTool;

namespace Silksprite.QuestReplacer.Drawers
{
    [CustomPropertyDrawer(typeof(QuestReplacerConfig))]
    public class QuestReplacerConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
        {
            var platform = serializedProperty.Lop(nameof(QuestReplacerConfig.platform), Loc("QuestReplacerConfig::platform"));
            var generateMode = serializedProperty.Lop(nameof(QuestReplacerConfig.materialGenerationMode), Loc("QuestReplacerConfig::materialGenerationMode"));
            var manageMaterials = serializedProperty.Lop(nameof(QuestReplacerConfig.manageMaterials), Loc("QuestReplacerConfig::manageMaterials"));
            var manageMeshes = serializedProperty.Lop(nameof(QuestReplacerConfig.manageMeshes), Loc("QuestReplacerConfig::manageMeshes"));
            var manageAnimationClips = serializedProperty.Lop(nameof(QuestReplacerConfig.manageAnimationClips), Loc("QuestReplacerConfig::manageAnimationClips"));
            var targetVRChatAnimations = serializedProperty.Lop(nameof(QuestReplacerConfig.targetVRChatAnimations), Loc("QuestReplacerConfig::targetVRChatAnimations"));
            
            position.height = EditorGUIUtility.singleLineHeight;
            LEditorGUI.PropAsEnumPopup<QuestReplacerPlatform>(position, platform);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            LEditorGUI.PropAsEnumPopup<QuestReplacerMaterialGenerationMode>(position, generateMode);
            EditorGUIUtility.labelWidth += 60f;
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            LEditorGUI.Prop(position, manageMaterials);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            LEditorGUI.Prop(position, manageMeshes);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            LEditorGUI.Prop(position, manageAnimationClips);
#if QUESTREPLACER_NDMF_SUPPORT
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            LEditorGUI.Prop(position, targetVRChatAnimations);
#endif
            EditorGUIUtility.labelWidth -= 60f;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
#if QUESTREPLACER_NDMF_SUPPORT
            const int propertyCount = 6;
#else
            const int propertyCount = 5;
#endif
            return EditorGUIUtility.singleLineHeight * propertyCount + EditorGUIUtility.standardVerticalSpacing * (propertyCount - 1);
        }
    }
}