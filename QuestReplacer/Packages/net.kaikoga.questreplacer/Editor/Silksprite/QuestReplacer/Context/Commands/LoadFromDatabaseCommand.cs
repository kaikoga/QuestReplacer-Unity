using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class LoadFromDatabaseCommand : CommandBase
    {
        public LoadFromDatabaseCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Load");
            Undo.RecordObject(questReplacer, "QuestReplacer: Load");
            var db = questReplacer.EnsureDatabase(null);
            questReplacer.pairs = questReplacer.pairs.Update(db.pairs).ToList();
            questReplacer.AddEntries(context.DeepCollectReferences<Object>(), db, false);
            UpdateTypeFilters(questReplacer, context);
        }
    }
}