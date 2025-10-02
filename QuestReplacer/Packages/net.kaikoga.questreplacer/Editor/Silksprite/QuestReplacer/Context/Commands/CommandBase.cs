using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public abstract class CommandBase
    {
        protected abstract string Name { get; }
        protected virtual bool InvalidateTypeFilters => true;

        protected readonly QuestReplacer QuestReplacer;
        QuestReplacerContext _context;
        QuestReplacerDatabase _database;
        AssetDuplicator<AnimationClip> _animationClipDuplicator;
        AssetDuplicator<Material> _materialDuplicator;

        protected QuestReplacerContext Context => _context ??= QuestReplacer.ToContext(false);

        protected CommandBase(QuestReplacer questReplacer)
        {
            QuestReplacer = questReplacer;
        }

        protected QuestReplacerDatabase EnsureDatabase()
        {
            if (_database) return _database;
            _database = QuestReplacer.EnsureDatabase(null);
            Undo.RecordObject(_database, Name);
            return _database;
        }

        protected AssetDuplicator<AnimationClip> GetAnimationClipAssetDuplicator()
        {
            return _animationClipDuplicator ??= EnsureDatabase()
                .CreateAnimationClipAssetDuplicator(QuestReplacerAnimationClipGenerationMode.Instantiate);
        }

        protected AssetDuplicator<Material> GetMaterialAssetDuplicator()
        {
            return _materialDuplicator ??= EnsureDatabase()
                .CreateMaterialAssetDuplicator(QuestReplacer.Config.materialGenerationMode);
        }

        public void Execute()
        {
            Undo.SetCurrentGroupName(Name);
            Undo.RecordObject(QuestReplacer, Name);
            DoExecute();
            if (InvalidateTypeFilters)
            {
                UpdateTypeFilters();
            }
        }

        internal void ExecuteWithoutUndo()
        {
            DoExecute();
        }

        protected abstract void DoExecute();

        void UpdateTypeFilters()
        {
            if (QuestReplacer.database is QuestReplacerDatabase database)
            {
                Undo.RecordObject(database, Name);
                database.RegisterTypeFilters(Context.DeepCollectComponentTypes());
            }
        }
    }
}