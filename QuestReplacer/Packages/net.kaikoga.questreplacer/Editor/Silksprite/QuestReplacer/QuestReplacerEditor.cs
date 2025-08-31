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
                                    GenerateMaterials();
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
                                GenerateAnimationClips();
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
            DoCollect<T>(_questReplacer, _context);
            RecreateContext();
        }

        static void DoCollect<T>(QuestReplacer questReplacer, QuestReplacerContext context) where T : Object
        {
            Undo.SetCurrentGroupName("QuestReplacer: Collect");
            Undo.RecordObject(questReplacer, "QuestReplacer: Collect");
            questReplacer.AddEntries(context.DeepCollectReferences<T>(), null, true);
            UpdateTypeFilters(questReplacer, context);
        }

        void CleanupPairs()
        {
            DoCleanupPairs(_questReplacer, _context);
            RecreateContext();
        }

        static void DoCleanupPairs(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Cleanup");
            Undo.RecordObject(questReplacer, "QuestReplacer: Cleanup");
            questReplacer.pairs = questReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
            UpdateTypeFilters(questReplacer, context);
        }

        void CreateDatabase()
        {
            DoCreateDatabase(_questReplacer, _context);
            RecreateContext();
        }

        static void DoCreateDatabase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
            UpdateTypeFilters(questReplacer, context);
        }

        void GenerateMaterials()
        {
            DoGenerateMaterials(_questReplacer, _context);
            RecreateContext();
        }

        static void DoGenerateMaterials(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var duplicator = questReplacer.EnsureDatabase(null).CreateMaterialAssetDuplicator(questReplacer.Config.materialGenerationMode);
            Undo.SetCurrentGroupName("QuestReplacer: Generate Materials");
            Undo.RecordObject(questReplacer, "QuestReplacer: Generate Materials");
            foreach (var pair in questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    var rightMaterial = duplicator.Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate Materials");
                    pair.right = rightMaterial; 
                }
            }
            UpdateTypeFilters(questReplacer, context);
        }

        void GenerateAnimationClips()
        {
            DoGenerateAnimationClips(_questReplacer, _context);
            RecreateContext();
        }

        static void DoGenerateAnimationClips(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var duplicator = questReplacer
                .EnsureDatabase(null)
                .CreateAnimationClipAssetDuplicator(QuestReplacerAnimationClipGenerationMode.Instantiate);
            Undo.SetCurrentGroupName("QuestReplacer: Generate AnimationClips");
            Undo.RecordObject(questReplacer, "QuestReplacer: Generate AnimationClips");
            foreach (var pair in questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is AnimationClip leftAnimationClip)
                {
                    var rightMaterial = duplicator.Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate AnimationClips");
                    pair.right = rightMaterial; 
                }
            }
            UpdateTypeFilters(questReplacer, context);
        }

        void LoadFromDatabase()
        {
            DoLoadFromDatabase(_questReplacer, _context);
            RecreateContext();
        }

        static void DoLoadFromDatabase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Load");
            Undo.RecordObject(questReplacer, "QuestReplacer: Load");
            var db = questReplacer.EnsureDatabase(null);
            questReplacer.pairs = questReplacer.pairs.Update(db.pairs).ToList();
            questReplacer.AddEntries(context.DeepCollectReferences<Object>(), db, false);
            UpdateTypeFilters(questReplacer, context);
        }

        void SaveToDatabase()
        {
            DoSaveToDatabase(_questReplacer, _context);
            RecreateContext();
        }

        static void DoSaveToDatabase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Save");
            var db = questReplacer.EnsureDatabase(null);
            Undo.RecordObject(db, "QuestReplacer: Save");
            db.pairs = db.pairs.Merge(questReplacer.pairs.Where(pair => !pair.LikelyUnset)).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
            UpdateTypeFilters(questReplacer, context);
        }

        void Convert(bool toRight)
        {
            DoConvert(toRight, _questReplacer, _context);
            RecreateContext();
        }

        static void DoConvert(bool toRight, QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName(toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            UpdateTypeFilters(questReplacer, context);
            context.DeepOverrideReferences<Object>(toRight, withAssets: false);
        }

        static void UpdateTypeFilters(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var db = questReplacer.database;
            if (db)
            {
                Undo.RecordObject(db, "QuestReplacer: Update Type Filters");
                db.RegisterTypeFilters(context.DeepCollectComponentTypes());
            }
        }
        
        void ResetConfig()
        {
            DoResetConfig(_questReplacer);
            RecreateContext();
        }

        static void DoResetConfig(QuestReplacer questReplacer)
        {

            Undo.SetCurrentGroupName("QuestReplacer: Reset Config");
            Undo.RecordObject(questReplacer, "QuestReplacer: Reset Config");
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(questReplacer.database.config), questReplacer.config);
        }
    }
}