using System.Linq;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CleanupPairsCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Cleanup";

        public CleanupPairsCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            questReplacer.pairs = questReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
        }
    }
}