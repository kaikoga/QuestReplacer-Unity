using System.Linq;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CleanupPairsCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Cleanup";

        public CleanupPairsCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            QuestReplacer.pairs = QuestReplacer.pairs.Where(pair => !pair.LikelyUnset).ToList();
        }
    }
}