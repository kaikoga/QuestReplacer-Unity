using System.Linq;
using nadena.dev.ndmf.runtime;
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
        bool _currentEnableNdmfSupport;
        Transform _currentAvatarRootTransform;
        bool _force;

#if QUESTREPLACER_NDMF_SUPPORT
        static bool HasNdmfSupport => true;
#else
        static bool HasNdmfSupport => false;
#endif
        
        bool EnableNdmfSupport
        {
            get => _currentEnableNdmfSupport;
            set
            {
                if (_currentEnableNdmfSupport == value) return;
                _currentEnableNdmfSupport = value;
                RecreateContext();
            }
        }

        
        Transform AvatarRootTransform
        {
            get => _currentAvatarRootTransform;
            set
            {
                if (_currentAvatarRootTransform == value) return;
                _currentAvatarRootTransform = value;
                RecreateContext();
            }
        }
        QuestReplacerContext _context;

        void RecreateContext()
        {
            _context = AvatarRootTransform ? _questReplacer.ToAvatarContext(AvatarRootTransform) : _questReplacer.ToContext();
        }

        SerializedProperty _serializedDatabase;
        SerializedProperty _serializedTargets;
        SerializedProperty _serializedTargetSceneObjects;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _questReplacer = (QuestReplacer)target;
#if QUESTREPLACER_NDMF_SUPPORT
            AvatarRootTransform = RuntimeUtil.FindAvatarInParents(_questReplacer.transform);
#endif
            RecreateContext();

            _serializedDatabase = serializedObject.FindProperty(nameof(QuestReplacer.database));
            _serializedTargets = serializedObject.FindProperty(nameof(QuestReplacer.targets));
            _serializedTargetSceneObjects = serializedObject.FindProperty(nameof(QuestReplacer.targetSceneObjects));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacer.pairs)));
            _serializedTargets.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
#if QUESTREPLACER_NDMF_SUPPORT
            AvatarRootTransform = RuntimeUtil.FindAvatarInParents(_questReplacer.transform);
#endif
            EnableNdmfSupport = HasNdmfSupport && AvatarRootTransform;
            var hasTargets = EnableNdmfSupport || _questReplacer.HasTargets;
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                if (!EnableNdmfSupport)
                {
                    using (new EditorGUI.DisabledScope(_serializedTargetSceneObjects.boolValue))
                    {
                        EditorGUILayout.PropertyField(_serializedTargets);
                    }
                    EditorGUILayout.PropertyField(_serializedTargetSceneObjects);
                }

                _reorderablePairs.DoLayoutList();

                var pairs = _questReplacer.pairs.ToArray();
                if (_questReplacer.ManageMaterials)
                {
                    using (new EditorGUI.DisabledScope(!hasTargets))
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Material Status", $"{_context.ToQuestStatus<Material>()}");
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
                        EditorGUILayout.LabelField("Quest Mesh Status", $"{_context.ToQuestStatus<Mesh>()}");
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
                    RecreateContext();
                }
                serializedObject.ApplyModifiedProperties();

                var isReversible = pairs.Validate(out var messages);
                var requireForce = false;
                if (!isReversible)
                {
                    EditorGUILayout.HelpBox($"置き換え設定を確認してください。\n{string.Join("\n", messages)}", MessageType.Error);
                    requireForce = true;
                }
                
                if (EnableNdmfSupport)
                {
                    EditorGUILayout.HelpBox("NDMF連携が有効です。置き換えの結果はNDMFプレビューとして表示されるため、手動置き換え操作は不要です。", MessageType.Info);
                    requireForce = true;
                }

                _force = requireForce && EditorGUILayout.Toggle("確認した", _force);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!(hasTargets && pairs.Length > 0)))
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
            RecreateContext();
        }

        void CreateDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            _questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerGenerateMode.GenerateVRChatToonStandard);
            UpdateTypeFilters();
            RecreateContext();
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
            RecreateContext();
        }

        void LoadFromDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Load");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Load");
            var db = _questReplacer.EnsureDatabase(null);
            _questReplacer.pairs = _questReplacer.pairs.Update(db.pairs).ToList();
            _questReplacer.AddEntries(_context.DeepCollectReferences<Object>(), db, false);
            UpdateTypeFilters();
            RecreateContext();
        }

        void SaveToDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Save");
            var db = _questReplacer.EnsureDatabase(null);
            Undo.RecordObject(db, "QuestReplacer: Save");
            db.pairs = db.pairs.Merge(_questReplacer.pairs).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
            UpdateTypeFilters();
            RecreateContext();
        }

        void Convert(bool toRight)
        {
            Undo.SetCurrentGroupName(toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            UpdateTypeFilters();
            _context.DeepOverrideReferences<Object>(toRight);
            RecreateContext();
        }
        
        void UpdateTypeFilters()
        {
            var db = _questReplacer.database; 
            Undo.RecordObject(db, "QuestReplacer: Update Type Filters");
            if (db) db.RegisterTypeFilters(_context.DeepCollectComponentTypes());
        }
    }
}