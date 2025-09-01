using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ResetConfigCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Reset Config";
        protected override bool InvalidateTypeFilters => false;

        public ResetConfigCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(QuestReplacer.database.config), QuestReplacer.config);
            QuestReplacer.hasOverrideConfig = true;
        }
    }
}