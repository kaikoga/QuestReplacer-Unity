using UnityEditor;

namespace Silksprite.QuestReplacer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestReplacerDatabase))]
    public class QuestReplacerDatabaseEditor : Editor
    {
        SerializedProperty _serializedManageMaterials;
        SerializedProperty _serializedManageMeshes;
        SerializedProperty _serializedGenerateMode;
        SerializedProperty _serializedGeneratedDirectory;
        SerializedProperty _serializedGeneratedFilePrefix;
        SerializedProperty _serializedGeneratedFileSuffix;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _serializedManageMaterials = serializedObject.FindProperty(nameof(QuestReplacerDatabase.manageMaterials));
            _serializedManageMeshes = serializedObject.FindProperty(nameof(QuestReplacerDatabase.manageMeshes));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacerDatabase.pairs)));
            _serializedGenerateMode = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generateMode));
            _serializedGeneratedDirectory = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedDirectory));
            _serializedGeneratedFilePrefix = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedFilePrefix));
            _serializedGeneratedFileSuffix = serializedObject.FindProperty(nameof(QuestReplacerDatabase.generatedFileSuffix));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_serializedManageMaterials);
            EditorGUILayout.PropertyField(_serializedManageMeshes);
            _reorderablePairs.DoLayoutList();
            EditorGUILayout.PropertyField(_serializedGenerateMode);
            EditorGUILayout.PropertyField(_serializedGeneratedDirectory);
            EditorGUILayout.PropertyField(_serializedGeneratedFilePrefix);
            EditorGUILayout.PropertyField(_serializedGeneratedFileSuffix);
            serializedObject.ApplyModifiedProperties();
        }
    }
}