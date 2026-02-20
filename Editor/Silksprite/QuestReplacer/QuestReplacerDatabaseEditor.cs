using Silksprite.Loch;
using Silksprite.Loch.Extensions;
using Silksprite.Loch.IMGUI;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using static Silksprite.Loch.Tools.LochTool;

namespace Silksprite.QuestReplacer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestReplacerDatabase))]
    public class QuestReplacerDatabaseEditor : Editor
    {
        QuestReplacerDatabase _database;

        LocalizedProperty _config;
        LocalizedProperty _generatedDirectory;
        LocalizedProperty _generatedFilePrefix;
        LocalizedProperty _generatedFileSuffix;
        QuestReplacementReorderableList _reorderablePairs;
        QuestReplacementReorderableList _reorderableComponentFilters;

        void OnEnable()
        {
            _database =  (QuestReplacerDatabase)target;
            _config = serializedObject.Lop(nameof(QuestReplacerDatabase.config), Loc("QuestReplacerDatabase::config"));
            _reorderableComponentFilters = new QuestReplacementReorderableList(serializedObject,
                serializedObject.Lop(nameof(QuestReplacerDatabase.componentFilters), Loc("QuestReplacerDatabase::componentFilters")));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject,
                serializedObject.Lop(nameof(QuestReplacerDatabase.pairs), Loc("QuestReplacerDatabase::pairs")));
            _generatedDirectory = serializedObject.Lop(nameof(QuestReplacerDatabase.generatedDirectory), Loc("QuestReplacerDatabase::generatedDirectory"));
            _generatedFilePrefix = serializedObject.Lop(nameof(QuestReplacerDatabase.generatedFilePrefix), Loc("QuestReplacerDatabase::generatedFilePrefix"));
            _generatedFileSuffix = serializedObject.Lop(nameof(QuestReplacerDatabase.generatedFileSuffix), Loc("QuestReplacerDatabase::generatedFileSuffix"));
        }

        public override void OnInspectorGUI()
        {
            LEditorGUILayout.LanguageSelector();
            LEditorGUILayout.Prop(_config);
            _reorderablePairs.DoLayoutList();
            _reorderableComponentFilters.DoLayoutList();
            var hasPlatformSupport = _database.HasGenerateModeSupport(); 
            if (!hasPlatformSupport)
            {
                LEditorGUILayout.HelpBox(Loc("QuestReplacerDatabase::PlatformSupportNotFound."), MessageType.Error);
            }

            LEditorGUILayout.Prop(_generatedDirectory);
            LEditorGUILayout.Prop(_generatedFilePrefix);
            LEditorGUILayout.Prop(_generatedFileSuffix);
            serializedObject.ApplyModifiedProperties();
        }
    }
}