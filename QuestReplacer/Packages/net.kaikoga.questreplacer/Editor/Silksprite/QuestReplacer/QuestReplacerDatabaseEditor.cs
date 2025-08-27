using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestReplacerDatabase))]
    public class QuestReplacerDatabaseEditor : Editor
    {
        QuestReplacerDatabase _database;

        SerializedProperty _serializedConfig;
        SerializedProperty _serializedGeneratedDirectory;
        SerializedProperty _serializedGeneratedFilePrefix;
        SerializedProperty _serializedGeneratedFileSuffix;
        QuestReplacementReorderableList _reorderablePairs;
        QuestReplacementReorderableList _reorderableComponentFilters;

        void OnEnable()
        {
            _database =  (QuestReplacerDatabase)target;
            _serializedConfig = serializedObject.FindProperty(nameof(QuestReplacerDatabase.config));
            _reorderableComponentFilters = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacerDatabase.componentFilters)));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacerDatabase.pairs)));
            _serializedGeneratedDirectory = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedDirectory));
            _serializedGeneratedFilePrefix = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedFilePrefix));
            _serializedGeneratedFileSuffix = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedFileSuffix));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_serializedConfig);
            _reorderablePairs.DoLayoutList();
            _reorderableComponentFilters.DoLayoutList();
            var hasPlatformSupport = _database.HasGenerateModeSupport(); 
            if (!hasPlatformSupport)
            {
                EditorGUILayout.HelpBox("マテリアルの自動変換に必要なライブラリがインポートされてないか、非対応の変換です。", MessageType.Error);
            }

            EditorGUILayout.PropertyField(_serializedGeneratedDirectory);
            EditorGUILayout.PropertyField(_serializedGeneratedFilePrefix);
            EditorGUILayout.PropertyField(_serializedGeneratedFileSuffix);
            serializedObject.ApplyModifiedProperties();
        }
    }
}