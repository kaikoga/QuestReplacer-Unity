using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacer : MonoBehaviour
#if QUESTREPLACER_VRCSDK3_AVATARS
        , VRC.SDKBase.IEditorOnly
#endif
    {
        public QuestReplacerDatabase database;
        
        [SerializeField] internal List<Transform> targets = new List<Transform>();
        [SerializeField] internal bool targetSceneObjects;

        public QuestReplacerPlatform Platform => Config.platform;

        [SerializeField] internal QuestReplacerConfig config = new QuestReplacerConfig();
        [SerializeField] internal bool overrideConfig;

        public QuestReplacerConfig Config => overrideConfig ? config : database?.config ?? config;

        public Transform Target
        {
            set
            {
                targets.Clear();
                if (value) targets.Add(value);
                targetSceneObjects = false;
            }
        }

        public IEnumerable<Transform> Targets
        {
            get
            {
                if (targetSceneObjects)
                {
                    return SceneManager.GetActiveScene().GetRootGameObjects().Select(root => root.transform);
                }

                return targets;
            }
        }
        

        public bool HasTargets => targets.Count > 0 || targetSceneObjects;
        
        public List<QuestReplacement> pairs = new List<QuestReplacement>();
        
        public void AddEntries(IEnumerable<Object> source, QuestReplacerDatabase db, bool defaultEntry)
        {
            foreach (var obj in source)
            {
                if (pairs.Any(r => r.Contains(obj))) continue;
                QuestReplacement replacement = null;
                if (db)
                {
                    replacement = db.pairs.FirstOrDefault(entry => entry.Contains(obj));
                }
                if (replacement == null && defaultEntry)
                {
                    replacement = new QuestReplacement
                    {
                        left = obj,
                        right = obj
                    };
                }
                if (replacement != null)
                {
                    pairs.Add(replacement);
                }
            }
        }
    }
}