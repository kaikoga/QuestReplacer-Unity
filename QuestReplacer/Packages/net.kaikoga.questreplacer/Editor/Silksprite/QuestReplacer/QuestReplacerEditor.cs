using System.Linq;
using Silksprite.QuestReplacer.Context;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
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
            _context?.Dispose();
            _context = _questReplacer.ToContext(false);
        }

        SerializedProperty _serializedConfig;
        SerializedProperty _serializedOverrideConfig;
        SerializedProperty _serializedDatabase;
        SerializedProperty _serializedTargets;
        SerializedProperty _serializedTargetSceneObjects;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _questReplacer = (QuestReplacer)target;
            AvatarRootTransform = _questReplacer.NdmfAvatarRootTransform();
            RecreateContext();

            _serializedConfig = serializedObject.FindProperty(nameof(QuestReplacer.config));
            _serializedOverrideConfig = serializedObject.FindProperty(nameof(QuestReplacer.overrideConfig));
            _serializedDatabase = serializedObject.FindProperty(nameof(QuestReplacer.database));
            _serializedTargets = serializedObject.FindProperty(nameof(QuestReplacer.targets));
            _serializedTargetSceneObjects = serializedObject.FindProperty(nameof(QuestReplacer.targetSceneObjects));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject, serializedObject.FindProperty(nameof(QuestReplacer.pairs)));
            _serializedTargets.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            var config = _questReplacer.Config;
            AvatarRootTransform = _questReplacer.NdmfAvatarRootTransform();
            var hasTargets = AvatarRootTransform || _questReplacer.HasTargets;
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                if (!AvatarRootTransform)
                {
                    using (new EditorGUI.DisabledScope(_serializedTargetSceneObjects.boolValue))
                    {
                        EditorGUILayout.PropertyField(_serializedTargets);
                    }
                    EditorGUILayout.PropertyField(_serializedTargetSceneObjects);
                }

                using (var duplicateButton = new ShowDuplicateButtonScope())
                {
                    _reorderablePairs.DoLayoutList();
                    if (duplicateButton.DuplicateButtonClicked(out var propertyPath))
                    {
                        Debug.LogError(serializedObject.FindProperty(propertyPath));
                    }
                }

                var pairs = _questReplacer.pairs.ToArray();
                if (config.manageMaterials)
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
                        if (_serializedDatabase.objectReferenceValue)
                        {
                            var db = _questReplacer.EnsureDatabase(null);
                            var hasPlatformSupport = db.HasGenerateModeSupport(); 
                            if (!hasPlatformSupport)
                            {
                                EditorGUILayout.HelpBox("マテリアルの自動変換に必要なライブラリがインポートされてないか、非対応の変換です。", MessageType.Error);
                            }
                            using (new EditorGUI.DisabledScope(!hasPlatformSupport))
                            {
                                if (GUILayout.Button($"{config.materialGenerationMode} Materials"))
                                {
                                    GenerateMaterials(db.CreateMaterialAssetDuplicator(config.materialGenerationMode));
                                }
                            }
                        }
                    }
                }
                if (config.manageMeshes)
                {
                    using (new EditorGUI.DisabledScope(!hasTargets))
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Mesh Status", $"{_context.ToQuestStatus<Mesh>()}");
                        if (GUILayout.Button("Collect"))
                        {
                            Collect<Mesh>();
                        }
                    }
                }

                if (config.manageAnimationClips)
                {
                    using (new EditorGUI.DisabledScope(!hasTargets))
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Animation Clip Status", $"{_context.ToQuestStatus<AnimationClip>()}");
                        if (GUILayout.Button("Collect"))
                        {
                            Collect<AnimationClip>();
                        }
                    }
                    
                    if (pairs.Any(pair => pair.LikelyUnset))
                    {
                        if (_serializedDatabase.objectReferenceValue)
                        {
                            if (GUILayout.Button("Instantiate Animation Clips"))
                            {
                                GenerateAnimationClips(_questReplacer
                                    .EnsureDatabase(null)
                                    .CreateAnimationClipAssetDuplicator(QuestReplacerAnimationClipGenerationMode.Instantiate));
                            }
                        }
                    }
                }

                if (GUILayout.Button("Cleanup"))
                {
                    CleanupPairs();
                }

                using (new BoxLayoutScope())
                {
                    EditorGUILayout.PropertyField(_serializedDatabase);
                    if (_serializedDatabase.objectReferenceValue)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
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
                    else
                    {
                        if (GUILayout.Button("Create"))
                        {
                            CreateDatabase();
                        }
                    }
                }
                EditorGUILayout.PropertyField(_serializedOverrideConfig);
                using (new BoxLayoutScope())
                {
                    if (_serializedOverrideConfig.boolValue)
                    {
                        EditorGUILayout.PropertyField(_serializedConfig);
                        if (GUILayout.Button("Reset"))
                        {
                            ResetConfig();
                        }
                    }
                    else if (_questReplacer.database)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            using var serializedDatabase = new SerializedObject(_questReplacer.database);
                            EditorGUILayout.PropertyField(serializedDatabase.FindProperty(nameof(QuestReplacerDatabase.config)));
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
                
                if (AvatarRootTransform)
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

        void CleanupPairs()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Cleanup");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Cleanup");
            _questReplacer.pairs = _questReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
            UpdateTypeFilters();
            RecreateContext();
        }

        void CreateDatabase()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            _questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
            UpdateTypeFilters();
            RecreateContext();
        }

        void GenerateMaterials(AssetDuplicator<Material> duplicator)
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

        void GenerateAnimationClips(AssetDuplicator<AnimationClip> duplicator)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Generate AnimationClips");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Generate AnimationClips");
            foreach (var pair in _questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is AnimationClip leftAnimationClip)
                {
                    var rightMaterial = duplicator.Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate AnimationClips");
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
            db.pairs = db.pairs.Merge(_questReplacer.pairs.Where(pair => !pair.LikelyUnset)).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
            UpdateTypeFilters();
            RecreateContext();
        }

        void Convert(bool toRight)
        {
            Undo.SetCurrentGroupName(toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            UpdateTypeFilters();
            _context.DeepOverrideReferences<Object>(toRight, withAssets: false);
            RecreateContext();
        }
        
        void UpdateTypeFilters()
        {
            var db = _questReplacer.database;
            if (db)
            {
                Undo.RecordObject(db, "QuestReplacer: Update Type Filters");
                db.RegisterTypeFilters(_context.DeepCollectComponentTypes());
            }
        }
        
        void ResetConfig()
        {
            Undo.SetCurrentGroupName("QuestReplacer: Reset Config");
            Undo.RecordObject(_questReplacer, "QuestReplacer: Reset Config");
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(_questReplacer.database.config), _questReplacer.config);
            RecreateContext();
        }
    }
}