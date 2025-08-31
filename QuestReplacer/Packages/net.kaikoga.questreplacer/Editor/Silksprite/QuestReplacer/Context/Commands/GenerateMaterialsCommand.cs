using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class GenerateMaterialsCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Generate Materials";

        public GenerateMaterialsCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            var duplicator = EnsureDatabase(false)
                .CreateMaterialAssetDuplicator(QuestReplacer.Config.materialGenerationMode);
            foreach (var pair in QuestReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    var rightMaterial = duplicator.Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate Materials");
                    pair.right = rightMaterial; 
                }
            }
        }
    }
}