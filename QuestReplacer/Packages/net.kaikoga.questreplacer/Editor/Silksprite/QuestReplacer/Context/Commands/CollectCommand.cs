using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CollectCommand<T> : CommandBase
    where T : Object
    {
        protected override string Name => "QuestReplacer: Collect";

        public CollectCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            QuestReplacer.AddEntries(Context.DeepCollectReferences<T>(), null, true);
        }
    }
}