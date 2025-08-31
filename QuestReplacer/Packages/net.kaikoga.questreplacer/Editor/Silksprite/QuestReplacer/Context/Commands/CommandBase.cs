using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public abstract class CommandBase
    {
        readonly QuestReplacer _questReplacer;
        readonly QuestReplacerContext _context;

        protected CommandBase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            _questReplacer = questReplacer;
            _context = context;
        }

        public void Execute() => DoExecute(_questReplacer, _context);

        protected abstract void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context);

        protected static void UpdateTypeFilters(QuestReplacer questReplacer, QuestReplacerContext context)
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