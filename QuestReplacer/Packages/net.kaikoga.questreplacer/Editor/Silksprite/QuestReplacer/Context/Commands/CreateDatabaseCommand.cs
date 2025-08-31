using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class CreateDatabaseCommand
    {
        public static void DoCreateDatabase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Create Database");
            questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}