using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CreateDatabaseCommand : CommandBase
    {
        public CreateDatabaseCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
            UpdateTypeFilters(questReplacer, context);
        }
    }
}