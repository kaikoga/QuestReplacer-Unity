using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CollectCommand<T> : CommandBase
    where T : Object
    {
        public CollectCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Collect");
            Undo.RecordObject(questReplacer, "QuestReplacer: Collect");
            questReplacer.AddEntries(context.DeepCollectReferences<T>(), null, true);
            UpdateTypeFilters(questReplacer, context);
        }
    }
}