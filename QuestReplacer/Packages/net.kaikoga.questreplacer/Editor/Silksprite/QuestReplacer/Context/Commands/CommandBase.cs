using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class CommandBase
    {
        public static void UpdateTypeFilters(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var db = questReplacer.database;
            if (db)
            {
                Undo.RecordObject(db, "QuestReplacer: Update Type Filters");
                db.RegisterTypeFilters(context.DeepCollectComponentTypes());
            }
        }
    }
}