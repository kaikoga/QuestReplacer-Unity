using System.Linq;
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
        bool _force;

        QuestReplacerContext _context;
        QuestReplacerContext Context
        {
            get
            {
                if (_context != null) return _context;

                _context = _questReplacer.ToContext();
                return _context;
            }
        }

        QuestStatus? _materialQuestStatus;
        QuestStatus? _meshQuestStatus;
        QuestStatus QuestMaterialStatus => _materialQuestStatus ?? (_materialQuestStatus = Context.ToQuestStatus<Material>()).Value;
        QuestStatus QuestMeshStatus => _meshQuestStatus ?? (_meshQuestStatus = Context.ToQuestStatus<Mesh>()).Value;


        void ClearCache()
        {
            _context = null;
            _materialQuestStatus = null;
            _meshQuestStatus = null;
        }

        SerializedProperty _serializedDatabase;
        SerializedProperty _serializedTargets;
        SerializedProperty _serializedTargetSceneObjects;
        QuestReplacementReorderableList _reorderablePairs;

        void OnEnable()
        {
            _questReplacer = (QuestReplacer)target;

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
                        EditorGUILayout.LabelField("Quest Material Status", $"{QuestMaterialStatus}");
                        if (GUILayout.Button("Collect"))
                        {
                            Collect<Material>();
                        }
                    }

                    if (pairs.Any(pair => pair.LikelyUnset))
                    {
                        if (_questReplacer.database is QuestReplacerDatabase database)
                        {
                            var hasPlatformSupport = database.HasPlatformSupport(); 
                            if (!hasPlatformSupport)
                            {
                                EditorGUILayout.HelpBox("マテリアルの自動変換に必要なライブラリがインポートされてないようです。", MessageType.Error);
                            }
                            using (new EditorGUI.DisabledScope(!hasPlatformSupport))
                            {
                                if (GUILayout.Button($"Generate {database.generateMode} Materials"))
                                {
                                    GenerateMaterials(_questReplacer.EnsureDatabase(QuestReplacerDatabase.Platform.VRChatMobile).CreateMaterialDuplicator());
                                }
                            }
                        }
                    }
                }
                if (_questReplacer.ManageMeshes)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Quest Mesh Status", $"{QuestMeshStatus}");
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
                if (!isReversible)
                {
                    EditorGUILayout.HelpBox($"置き換え設定を確認してください。\n{string.Join("\n", messages)}", MessageType.Error);
                    _force = EditorGUILayout.Toggle("確認した", _force);
                }
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!(_questReplacer.HasTargets && pairs.Length > 0)))
                    using (new EditorGUI.DisabledScope(!(isReversible || _force)))
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
            var db = _questReplacer.database;
            _questReplacer.AddEntries(Context.DeepCollectReferences<T>(), db, true);
            UpdateTypeFilters();
            ClearCache();
        }

        void CreateDatabase()
        {
            _questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRChatMobile, QuestReplacerDatabase.GenerateMode.Quest);
            UpdateTypeFilters();
            ClearCache();
        }

        void GenerateMaterials(MaterialDuplicator duplicator)
        {
            foreach (var pair in _questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    pair.right = duplicator.Duplicate(leftMaterial);
                }
            }
            UpdateTypeFilters();
            ClearCache();
        }

        void LoadFromDatabase()
        {
            var db = _questReplacer.EnsureDatabase(null);
            _questReplacer.pairs = _questReplacer.pairs.Update(db.pairs).ToList();
            _questReplacer.AddEntries(Context.DeepCollectReferences<Object>(), db, false);
            UpdateTypeFilters();
            ClearCache();
        }

        void SaveToDatabase()
        {
            var db = _questReplacer.EnsureDatabase(null);
            db.pairs = db.pairs.Merge(_questReplacer.pairs).ToList();
            UpdateTypeFilters();
            ClearCache();
        }

        void Convert(bool toRight)
        {
            UpdateTypeFilters();
            Context.DeepOverrideReferences<Object>(toRight);
            ClearCache();
        }
        
        void UpdateTypeFilters()
        {
            var db = _questReplacer.database; 
            if (db) db.RegisterTypeFilters(Context.DeepCollectComponentTypes());
        }
    }
}