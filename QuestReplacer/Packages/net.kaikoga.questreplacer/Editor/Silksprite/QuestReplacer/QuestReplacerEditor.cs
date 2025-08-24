using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.Scopes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestReplacer))]
    public class QuestReplacerEditor : Editor
    {
        QuestReplacer _questReplacer;
        bool _force;

        QuestReplacerReusableContext _context;

#if QUESTREPLACER_NDMF_SUPPORT
        const bool HasNdmfSupport = true;
#else
        const bool HasNdmfSupport = false;
#endif

        void ClearCache()
        {
            _context.SetDirty();
        }

        SerializedProperty _serializedDatabase;
        SerializedProperty _serializedTargets;
        SerializedProperty _serializedTargetSceneObjects;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _questReplacer = (QuestReplacer)target;
            _context = _questReplacer.ToReusableContext();

            _serializedDatabase = serializedObject.FindProperty(nameof(QuestReplacer.database));
            _serializedTargets = serializedObject.FindProperty(nameof(QuestReplacer.targets));
            _serializedTargetSceneObjects = serializedObject.FindProperty(nameof(QuestReplacer.targetSceneObjects));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacer.pairs)));
            _serializedTargets.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUI.DisabledScope(_serializedTargetSceneObjects.boolValue))
                {
                    EditorGUILayout.PropertyField(_serializedTargets);
                }
                EditorGUILayout.PropertyField(_serializedTargetSceneObjects);
                _reorderablePairs.DoLayoutList();

                var pairs = _questReplacer.pairs.ToArray();
                if (_questReplacer.ManageMaterials)
                {
                    using (new EditorGUI.DisabledScope(!_questReplacer.HasTargets))
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Material Status", $"{_context.QuestMaterialStatus}");
                        if (GUILayout.Button("Collect"))
                        {
                            Collect<Material>();
                        }
                    }

                    if (pairs.Any(pair => pair.LikelyUnset))
                    {
                        if (_questReplacer.database is QuestReplacerDatabase database)
                        {
                            var hasPlatformSupport = database.HasGenerateModeSupport(); 
                            if (!hasPlatformSupport)
                            {
                                EditorGUILayout.HelpBox("マテリアルの自動変換に必要なライブラリがインポートされてないか、非対応の変換です。", MessageType.Error);
                            }
                            using (new EditorGUI.DisabledScope(!hasPlatformSupport))
                            {
                                if (GUILayout.Button($"{database.generateMode} Materials"))
                                {
                                    GenerateMaterials(_questReplacer.EnsureDatabase(QuestReplacerPlatform.VRChatMobile).CreateMaterialDuplicator());
                                }
                            }
                        }
                    }
                }
                if (_questReplacer.ManageMeshes)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Mesh Status", $"{_context.QuestMeshStatus}");
                        if (GUILayout.Button("Collect"))
                        {
                            Collect<Mesh>();
                        }
                    }
                }

                using (new BoxLayoutScope())
                {
                    EditorGUILayout.PropertyField(_serializedDatabase);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (!_serializedDatabase.objectReferenceValue && GUILayout.Button("Create"))
                        {
                            CreateDatabase();
                        }
                        if (GUILayout.Button("Load"))
                        {
                            LoadFromDatabase();
                        }
                        if (GUILayout.Button("Save"))
                        {
                            SaveToDatabase();
                        }
                    }
                }

                if (changed.changed)
                {
                    ClearCache();
                }
                serializedObject.ApplyModifiedProperties();

                var isReversible = pairs.Validate(out var messages);
                var requireForce = false;
                if (!isReversible)
                {
                    EditorGUILayout.HelpBox($"置き換え設定を確認してください。\n{string.Join("\n", messages)}", MessageType.Error);
                    requireForce = true;
                }
                
                if (HasNdmfSupport)
                {
                    EditorGUILayout.HelpBox("ndmf 連携が有効です。", MessageType.Info);
                    requireForce = true;
                }

                _force = requireForce && EditorGUILayout.Toggle("確認した", _force);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!(_questReplacer.HasTargets && pairs.Length > 0)))
                    using (new EditorGUI.DisabledScope(requireForce != _force))
                    {
                        if (GUILayout.Button("To Left"))
                        {
                            Convert(false);
                        }
                        if (GUILayout.Button("To Right"))
                        {
                            Convert(true);
                        }
                    }
                }
            }
        }

        void Collect<T>()
        where T : Object
        {
            Undo.SetCurrentGroupName("QuestReplacer: Collect");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Collect");
            _questReplacer.AddEntries(_context.DeepCollectReferences<T>(), null, true);
            UpdateTypeFilters();
            ClearCache();
        }

        void CreateDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            _questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerGenerateMode.GenerateVRChatToonStandard);
            UpdateTypeFilters();
            ClearCache();
        }

        void GenerateMaterials(MaterialDuplicator duplicator)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Generate Materials");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Generate Materials");
            foreach (var pair in _questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    var rightMaterial = duplicator.Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate Materials");
                    pair.right = rightMaterial; 
                }
            }
            UpdateTypeFilters();
            ClearCache();
        }

        void LoadFromDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Load");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Load");
            var db = _questReplacer.EnsureDatabase(null);
            _questReplacer.pairs = _questReplacer.pairs.Update(db.pairs).ToList();
            _questReplacer.AddEntries(_context.DeepCollectReferences<Object>(), db, false);
            UpdateTypeFilters();
            ClearCache();
        }

        void SaveToDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Save");
            var db = _questReplacer.EnsureDatabase(null);
            Undo.RecordObject(db, "QuestReplacer: Save");
            db.pairs = db.pairs.Merge(_questReplacer.pairs).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
            UpdateTypeFilters();
            ClearCache();
        }

        void Convert(bool toRight)
        {
            Undo.SetCurrentGroupName(toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            UpdateTypeFilters();
            _context.DeepOverrideReferences<Object>(toRight);
            ClearCache();
        }
        
        void UpdateTypeFilters()
        {
            var db = _questReplacer.database; 
            Undo.RecordObject(db, "QuestReplacer: Update Type Filters");
            if (db) db.RegisterTypeFilters(_context.DeepCollectComponentTypes());
        }
    }
}