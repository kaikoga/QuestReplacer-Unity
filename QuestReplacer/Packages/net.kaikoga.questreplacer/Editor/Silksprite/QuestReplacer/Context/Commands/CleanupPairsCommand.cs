using System.Linq;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CleanupPairsCommand : CommandBase
    {
        public CleanupPairsCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Cleanup");
            Undo.RecordObject(questReplacer, "QuestReplacer: Cleanup");
            questReplacer.pairs = questReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
            UpdateTypeFilters(questReplacer, context);
        }
    }
}