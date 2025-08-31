using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CreateDatabaseCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Create Database";

        public CreateDatabaseCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
        }
    }
}