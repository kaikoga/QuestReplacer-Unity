using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public abstract class CommandBase
    {
        protected abstract string Name { get; }
        protected virtual bool InvalidateTypeFilters => true;

        protected readonly QuestReplacer QuestReplacer;
        QuestReplacerContext _context;
        QuestReplacerDatabase _database;

        protected QuestReplacerContext Context => _context ??= QuestReplacer.ToContext(false);

        protected CommandBase(QuestReplacer questReplacer)
        {
            QuestReplacer = questReplacer;
        }


        protected QuestReplacerDatabase EnsureDatabase(bool forUpdate)
        {
            if (_database) return _database;
            _database = QuestReplacer.EnsureDatabase(null);
            if (forUpdate) Undo.RecordObject(_database, Name);
            return _database;

        }

        public void Execute()
        {
            Undo.SetCurrentGroupName(Name);
            Undo.RecordObject(QuestReplacer, Name);
            DoExecute();
            if (InvalidateTypeFilters)
            {
                UpdateTypeFilters(QuestReplacer, _context);
            }
        }

        protected abstract void DoExecute();

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