using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class SaveToDatabaseCommand
    {
        public static void DoSaveToDatabase(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName("QuestReplacer: Save");
            var db = questReplacer.EnsureDatabase(null);
            Undo.RecordObject(db, "QuestReplacer: Save");
            db.pairs = db.pairs.Merge(questReplacer.pairs.Where(pair => !pair.LikelyUnset)).ToList();
            AssetDatabase.SaveAssetIfDirty(db);
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}