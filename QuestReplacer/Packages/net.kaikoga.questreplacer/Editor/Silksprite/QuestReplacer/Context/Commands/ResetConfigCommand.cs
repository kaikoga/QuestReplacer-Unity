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
            var database = QuestReplacer.database;
            var databaseConfig = database ? database.config : new QuestReplacerConfig();
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(databaseConfig), QuestReplacer.config);
            QuestReplacer.hasOverrideConfig = true;
        }
    }
}