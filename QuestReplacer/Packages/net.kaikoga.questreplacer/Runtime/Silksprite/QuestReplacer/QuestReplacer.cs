using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacer : MonoBehaviour
    {
        public QuestReplacerDatabase database;
        public Transform avatarRoot;
        public List<QuestReplacement> pairs = new List<QuestReplacement>();
        
        // ReSharper disable SimplifyConditionalTernaryExpression
        public bool ManageMaterials => database ? database.manageMaterials : QuestReplacerDatabase.ManageMaterialsDefault;
        public bool ManageMeshes => database ? database.manageMeshes : QuestReplacerDatabase.ManageMeshesDefault;
        // ReSharper enable SimplifyConditionalTernaryExpression

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