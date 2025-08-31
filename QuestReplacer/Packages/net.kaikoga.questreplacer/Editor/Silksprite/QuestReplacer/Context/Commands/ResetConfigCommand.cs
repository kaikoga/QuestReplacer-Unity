using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ResetConfigCommand : CommandBase
    {
        public ResetConfigCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Reset Config");
            Undo.RecordObject(questReplacer, "QuestReplacer: Reset Config");
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(questReplacer.database.config), questReplacer.config);
        }
    }
}