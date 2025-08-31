using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class CollectCommand
    {
        public static void DoCollect<T>(QuestReplacer questReplacer, QuestReplacerContext context) where T : Object
        {
            Undo.SetCurrentGroupName("QuestReplacer: Collect");
            Undo.RecordObject(questReplacer, "QuestReplacer: Collect");
            questReplacer.AddEntries(context.DeepCollectReferences<T>(), null, true);
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}