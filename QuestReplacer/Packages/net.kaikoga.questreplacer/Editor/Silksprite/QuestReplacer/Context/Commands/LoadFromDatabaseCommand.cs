using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class LoadFromDatabaseCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Load";

        public LoadFromDatabaseCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            var db = EnsureDatabase(false);
            QuestReplacer.pairs = QuestReplacer.pairs.Update(db.pairs).ToList();
            QuestReplacer.AddEntries(Context.DeepCollectReferences<Object>(), db, false);
        }
    }
}