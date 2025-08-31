using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class ResetConfigCommand
    {
        public static void DoResetConfig(QuestReplacer questReplacer)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Reset Config");
            Undo.RecordObject(questReplacer, "QuestReplacer: Reset Config");
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(questReplacer.database.config), questReplacer.config);
        }
    }
}