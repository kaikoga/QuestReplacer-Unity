using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CollectCommand<T> : CommandBase
    where T : Object
    {
        protected override string Name => "QuestReplacer: Collect";

        public CollectCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            questReplacer.AddEntries(context.DeepCollectReferences<T>(), null, true);
        }
    }
}