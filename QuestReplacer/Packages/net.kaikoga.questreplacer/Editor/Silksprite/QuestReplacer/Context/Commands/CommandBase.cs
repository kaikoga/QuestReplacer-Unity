using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public abstract class CommandBase
    {
        protected abstract string Name { get; }
        protected virtual bool InvalidateTypeFilters => true;

        readonly QuestReplacer _questReplacer;
        readonly QuestReplacerContext _context;
        QuestReplacerDatabase _database;

        protected CommandBase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            _questReplacer = questReplacer;
            _context = context;
        }

        protected QuestReplacerDatabase EnsureDatabase(bool forUpdate)
        {
            if (_database) return _database;
            _database = _questReplacer.EnsureDatabase(null);
            if (forUpdate) Undo.RecordObject(_database, Name);
            return _database;

        }

        public void Execute()
        {
            Undo.SetCurrentGroupName(Name);
            Undo.RecordObject(_questReplacer, Name);
            DoExecute(_questReplacer, _context);
            if (InvalidateTypeFilters)
            {
                UpdateTypeFilters(_questReplacer, _context);
            }
        }

        protected abstract void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context);

        protected void UpdateTypeFilters(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var db = questReplacer.database;
            if (db)
            {
                Undo.RecordObject(db, Name);
                db.RegisterTypeFilters(context.DeepCollectComponentTypes());
            }
        }
    }
}