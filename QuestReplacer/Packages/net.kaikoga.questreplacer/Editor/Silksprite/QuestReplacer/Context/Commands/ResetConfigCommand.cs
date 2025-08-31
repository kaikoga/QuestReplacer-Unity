using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ResetConfigCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Reset Config";
        protected override bool InvalidateTypeFilters => false;

        public ResetConfigCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(questReplacer.database.config), questReplacer.config);
        }
    }
}