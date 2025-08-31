using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class GenerateMaterialsCommand
    {
        public static void DoGenerateMaterials(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var duplicator = questReplacer.EnsureDatabase(null).CreateMaterialAssetDuplicator(questReplacer.Config.materialGenerationMode);
            Undo.SetCurrentGroupName("QuestReplacer: Generate Materials");
            Undo.RecordObject(questReplacer, "QuestReplacer: Generate Materials");
            foreach (var pair in questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    var rightMaterial = duplicator.Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate Materials");
                    pair.right = rightMaterial; 
                }
            }
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}