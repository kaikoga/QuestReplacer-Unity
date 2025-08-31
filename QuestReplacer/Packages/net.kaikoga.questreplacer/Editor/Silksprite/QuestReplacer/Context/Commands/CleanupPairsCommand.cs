using System.Linq;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class CleanupPairsCommand
    {
        public static void DoCleanupPairs(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Cleanup");
            Undo.RecordObject(questReplacer, "QuestReplacer: Cleanup");
            questReplacer.pairs = questReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}