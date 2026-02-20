using System;
using Silksprite.Loch;
using Silksprite.Loch.Extensions;
using Silksprite.Loch.IMGUI;
using Silksprite.QuestReplacer.Context;
using Silksprite.QuestReplacer.Context.Commands;
using Silksprite.QuestReplacer.Drawers;
using Silksprite.QuestReplacer.Extensions;
using Silksprite.QuestReplacer.Scopes;
using UnityEditor;
using UnityEngine;
using static Silksprite.Loch.Tools.LochTool;

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

        LocalizedProperty _config;
        LocalizedProperty _hasOverrideConfig;
        LocalizedProperty _overrideConfig;
        LocalizedProperty _database;
        LocalizedProperty _targets;
        LocalizedProperty _targetSceneObjects;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _questReplacer = (QuestReplacer)target;
            AvatarRootTransform = _questReplacer.NdmfAvatarRootTransform();
            RecreateContext();

            _config = serializedObject.Lop(nameof(QuestReplacer.config), Loc("QuestReplacer::config"));
            _hasOverrideConfig = serializedObject.Lop(nameof(QuestReplacer.hasOverrideConfig), Loc("QuestReplacer::hasOverrideConfig"));
            _overrideConfig = serializedObject.Lop(nameof(QuestReplacer.overrideConfig), Loc("QuestReplacer::overrideConfig"));
            _database = serializedObject.Lop(nameof(QuestReplacer.database), Loc("QuestReplacer::database"));
            _targets = serializedObject.Lop(nameof(QuestReplacer.targets), Loc("QuestReplacer::targets"));
            _targetSceneObjects = serializedObject.Lop(nameof(QuestReplacer.targetSceneObjects), Loc("QuestReplacer::targetSceneObjects"));
            _reorderablePairs = new QuestReplacementReorderableList(serializedObject,
                serializedObject.Lop(nameof(QuestReplacer.pairs), Loc("QuestReplacer::Pairs")));
            _targets.Property.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            LEditorGUILayout.LanguageSelector();
            var config = _questReplacer.Config;
            AvatarRootTransform = _questReplacer.NdmfAvatarRootTransform();
            var hasTargets = AvatarRootTransform || _questReplacer.HasTargets;
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                if (!AvatarRootTransform)
                {
                    using (new EditorGUI.DisabledScope(_targetSceneObjects.Property.boolValue))
                    {
                        LEditorGUILayout.Prop(_targets);
                    }
                    LEditorGUILayout.Prop(_targetSceneObjects);
                }

                LGUILayout.Header(Loc("QuestReplacer::database"));
                using (new BoxLayoutScope())
                {
                    LEditorGUILayout.Prop(_database);
                    if (_database.Property.objectReferenceValue)
                    {
                        if (_database.Property.objectReferenceValue != _questReplacer.database)
                        {
                            _hasOverrideConfig.Property.boolValue = false;
                            // FIXME: we need a RecreateContext() here, maybe defer commands
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            CommandButton(Loc("QuestReplacer::Load"), () => new LoadFromDatabaseCommand(_questReplacer));
                            CommandButton(Loc("QuestReplacer::Save"), () => new SaveToDatabaseCommand(_questReplacer));
                        }
                    }
                    else
                    {
                        CommandButton(Loc("QuestReplacer::Create"), () => new CreateDatabaseCommand(_questReplacer));
                    }
                }

                LGUILayout.Header(Loc("QuestReplacer::config"));
                LEditorGUILayout.Prop(_overrideConfig);
                if (_overrideConfig.Property.boolValue && !_hasOverrideConfig.Property.boolValue)
                {
                    new ResetConfigCommand(_questReplacer).Execute();
                    RecreateContext();
                }
                using (new BoxLayoutScope())
                {
                    if (_overrideConfig.Property.boolValue)
                    {
                        LEditorGUILayout.Prop(_config);
                        CommandButton(Loc("QuestReplacer::Reset"), () => new ResetConfigCommand(_questReplacer));
                    }
                    else if (_database.Property.objectReferenceValue is QuestReplacerDatabase database)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            using var serializedDatabase = new SerializedObject(database);
                            EditorGUILayout.PropertyField(serializedDatabase.FindProperty(nameof(QuestReplacerDatabase.config)));
                        }
                    }
                }

                if (_database.Property.objectReferenceValue)
                {
                    using (new EditorGUI.DisabledScope(!hasTargets))
                    {
                        CommandButton(Loc("QuestReplacer::SyncAndGenerate"), () => new SyncCommand(_questReplacer));
                    }
                    EditorGUILayout.Separator();
                    using (var duplicateButton = new ShowDuplicateButtonScope())
                    {
                        _reorderablePairs.DoLayoutList();
                        if (duplicateButton.DuplicateButtonClicked(out var index))
                        {
                            new GenerateSingleCommand(_questReplacer, index).Execute();
                            RecreateContext();
                        }
                    }
                }
                else
                {
                    EditorGUILayout.Separator();
                    _reorderablePairs.DoLayoutList();
                }

                CommandButton(Loc("QuestReplacer::Cleanup"), () => new CleanupPairsCommand(_questReplacer));

                LGUILayout.Header(Loc("QuestReplacer::assets"));
                if (config.manageMaterials)
                {
                    using (new BoxLayoutScope())
                    {
                        LGUILayout.Header(Loc("QuestReplacer::materials"));
                        LEditorGUILayout.LabelField(Loc("QuestReplacer::questStatus"), $"{_context.ToQuestStatus<Material>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton(Loc("QuestReplacer::Collect"), () => new CollectCommand<Material>(_questReplacer));
                        }

                        if (_database.Property.objectReferenceValue && _questReplacer.HasLikelyUnset<Material>())
                        {
                            var hasPlatformSupport = _questReplacer.EnsureDatabase(null).HasGenerateModeSupport();
                            if (!hasPlatformSupport)
                            {
                                LEditorGUILayout.HelpBox(Loc("QuestReplacerDatabase::PlatformSupportNotFound."), MessageType.Error);
                            }
                            using (new EditorGUI.DisabledScope(!hasPlatformSupport))
                            {
                                CommandButton(Loc("QuestReplacer::GenerateMaterials"),
                                    new Substitution
                                    {
                                        ["materialGenerationMode"] = config.materialGenerationMode.ToString()
                                    },
                                    () => new GenerateMaterialsCommand(_questReplacer));
                            }
                        }
                    }
                }
                if (config.manageMeshes)
                {
                    using (new BoxLayoutScope())
                    {
                        LGUILayout.Header(Loc("QuestReplacer::meshes"));
                        LEditorGUILayout.LabelField(Loc("QuestReplacer::questStatus"), $"{_context.ToQuestStatus<Mesh>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton(Loc("QuestReplacer::Collect"), () => new CollectCommand<Mesh>(_questReplacer));
                        }
                    }
                }

                if (config.manageAnimationClips)
                {
                    using (new BoxLayoutScope())
                    {
                        LGUILayout.Header(Loc("QuestReplacer::animationClips"));
                        LEditorGUILayout.LabelField(Loc("QuestReplacer::questStatus"), $"{_context.ToQuestStatus<AnimationClip>()}");

                        using (new EditorGUI.DisabledScope(!hasTargets))
                        {
                            CommandButton(Loc("QuestReplacer::Collect"), () => new CollectCommand<AnimationClip>(_questReplacer));
                        }

                        if (_database.Property.objectReferenceValue && _questReplacer.HasLikelyUnset<AnimationClip>())
                        {
                            CommandButton(Loc("QuestReplacer::InstantiateAnimationClips"), () => new GenerateAnimationClipsCommand(_questReplacer));
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
                EditorGUILayout.HelpBox($"{Loc("QuestReplacer::ConfirmSettings.").Tr}\n{string.Join("\n", messages)}".Nowrap(), MessageType.Error);
                requireForce = true;
            }
            
            if (AvatarRootTransform)
            {
                LEditorGUILayout.HelpBox(Loc("QuestReplacer::NdmfEnabled."), MessageType.Info);
                requireForce = true;
            }

            _force = requireForce && LEditorGUILayout.Toggle(Loc("QuestReplacer::force"), _force);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!(hasTargets && _questReplacer.pairs.Count > 0)))
                using (new EditorGUI.DisabledScope(requireForce != _force))
                {
                    CommandButton(Loc("QuestReplacer.ToLeft"), () => new ConvertCommand(_questReplacer, false));
                    CommandButton(Loc("QuestReplacer.ToRight"), () => new ConvertCommand(_questReplacer, true));
                }
            }
        }

        void CommandButton(LocalizedContent loc, Func<CommandBase> command)
        {
            if (LGUILayout.Button(loc))
            {
                command().Execute();
                RecreateContext();
            }
        }

        void CommandButton(LocalizedContent loc, Substitution sub, Func<CommandBase> command)
        {
            if (LGUILayout.Button(loc, sub))
            {
                command().Execute();
                RecreateContext();
            }
        }
    }
}