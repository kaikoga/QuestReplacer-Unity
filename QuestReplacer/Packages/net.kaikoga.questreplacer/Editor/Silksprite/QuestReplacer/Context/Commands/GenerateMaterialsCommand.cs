using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class GenerateMaterialsCommand : CommandBase
    {
        public GenerateMaterialsCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
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
            UpdateTypeFilters(questReplacer, context);
        }
    }
}