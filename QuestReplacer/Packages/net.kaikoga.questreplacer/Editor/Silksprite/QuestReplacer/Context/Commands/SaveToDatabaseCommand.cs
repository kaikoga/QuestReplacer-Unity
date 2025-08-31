using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class SaveToDatabaseCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Reset Save";
        public SaveToDatabaseCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            var db = EnsureDatabase(true);
            db.pairs = db.pairs.Merge(QuestReplacer.pairs.Where(pair => !pair.LikelyUnset)).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
        }
    }
}