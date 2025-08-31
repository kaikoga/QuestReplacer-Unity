using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class LoadFromDatabaseCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Load";

        public LoadFromDatabaseCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var db = EnsureDatabase(false);
            questReplacer.pairs = questReplacer.pairs.Update(db.pairs).ToList();
            questReplacer.AddEntries(context.DeepCollectReferences<Object>(), db, false);
        }
    }
}