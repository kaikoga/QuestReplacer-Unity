using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CreateDatabaseCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Create Database";

        public CreateDatabaseCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            QuestReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
        }
    }
}