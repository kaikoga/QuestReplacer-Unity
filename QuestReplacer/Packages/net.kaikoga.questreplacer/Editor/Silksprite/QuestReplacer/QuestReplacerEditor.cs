using System;
using Silksprite.QuestReplacer.Context;
using Silksprite.QuestReplacer.Context.Commands;
using Silksprite.QuestReplacer.Drawers;
using Silksprite.QuestReplacer.Extensions;
using Silksprite.QuestReplacer.Scopes;
using UnityEditor;
using UnityEngine;

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
        SerializedProperty _serializedHasOverrideConfig;
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
            _serializedHasOverrideConfig = serializedObject.FindProperty(nameof(QuestReplacer.hasOverrideConfig));
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
                    if (duplicateButton.DuplicateButtonClicked(out var index))
                    {
                        new GenerateSingleCommand(_questReplacer, index).Execute();
                        RecreateContext();
                    }
                }

                CommandButton("Cleanup", () => new CleanupPairsCommand(_questReplacer));

                if (config.manageMaterials)
                {
                    using (new BoxLayoutScope())
                    {
                        QuestReplacerGUILayout.Header("Materials");
                        EditorGUILayout.LabelField("Quest Status", $"{_context.ToQuestStatus<Material>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton("Collect", () => new CollectCommand<Material>(_questReplacer));
                        }

                        if (_serializedDatabase.objectReferenceValue && _questReplacer.HasLikelyUnset<Material>())
                        {
                            var hasPlatformSupport = _questReplacer.EnsureDatabase(null).HasGenerateModeSupport();
                            if (!hasPlatformSupport)
                            {
                                EditorGUILayout.HelpBox("マテリアルの自動変換に必要なライブラリがインポートされてないか、非対応の変換です。", MessageType.Error);
                            }
                            using (new EditorGUI.DisabledScope(!hasPlatformSupport))
                            {
                                CommandButton($"{config.materialGenerationMode} Materials", () => new GenerateMaterialsCommand(_questReplacer));
                            }
                        }
                    }
                }
                if (config.manageMeshes)
                {
                    using (new BoxLayoutScope())
                    {
                        QuestReplacerGUILayout.Header("Meshes");
                        EditorGUILayout.LabelField("Quest Status", $"{_context.ToQuestStatus<Mesh>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton("Collect", () => new CollectCommand<Mesh>(_questReplacer));
                        }
                    }
                }

                if (config.manageAnimationClips)
                {
                    using (new BoxLayoutScope())
                    {
                        QuestReplacerGUILayout.Header("AnimationClips");
                        EditorGUILayout.LabelField("Quest Status", $"{_context.ToQuestStatus<AnimationClip>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton("Collect", () => new CollectCommand<AnimationClip>(_questReplacer));
                        }

                        if (_serializedDatabase.objectReferenceValue && _questReplacer.HasLikelyUnset<AnimationClip>())
                        {
                            CommandButton("Instantiate Animation Clips", () => new GenerateAnimationClipsCommand(_questReplacer));
                        }
                    }
                }

                using (new BoxLayoutScope())
                {
                    EditorGUILayout.PropertyField(_serializedDatabase);
                    if (_serializedDatabase.objectReferenceValue)
                    {
                        if (_serializedDatabase.objectReferenceValue != _questReplacer.database)
                        {
                            _serializedHasOverrideConfig.boolValue = false;
                            // FIXME: we need a RecreateContext() here, maybe defer commands
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            CommandButton("Load", () => new LoadFromDatabaseCommand(_questReplacer));
                            CommandButton("Save", () => new SaveToDatabaseCommand(_questReplacer));
                        }
                    }
                    else
                    {
                        CommandButton("Create", () => new CreateDatabaseCommand(_questReplacer));
                    }
                }
                EditorGUILayout.PropertyField(_serializedOverrideConfig);
                if (_serializedOverrideConfig.boolValue && !_serializedHasOverrideConfig.boolValue)
                {
                    new ResetConfigCommand(_questReplacer).Execute();
                    RecreateContext();
                }
                using (new BoxLayoutScope())
                {
                    if (_serializedOverrideConfig.boolValue)
                    {
                        EditorGUILayout.PropertyField(_serializedConfig);
                        CommandButton("Reset", () => new ResetConfigCommand(_questReplacer));
                    }
                    else if (_serializedDatabase.objectReferenceValue)
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
                    serializedObject.ApplyModifiedProperties();
                    RecreateContext();
                }
                else
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            var isReversible = _questReplacer.pairs.Validate(out var messages);
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
                using (new EditorGUI.DisabledScope(!(hasTargets && _questReplacer.pairs.Count > 0)))
                using (new EditorGUI.DisabledScope(requireForce != _force))
                {
                    CommandButton("To Left", () => new ConvertCommand(_questReplacer, false));
                    CommandButton("To Right", () => new ConvertCommand(_questReplacer, true));
                }
            }
        }

        void CommandButton(string label, Func<CommandBase> command)
        {
            if (GUILayout.Button(label))
            {
                command().Execute();
                RecreateContext();
            }
        }
    }
}