using System.Linq;
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
            foreach (var pair in QuestReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is Material leftMaterial)
                {
                    var rightMaterial = GetMaterialAssetDuplicator().Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, Name);
                    pair.right = rightMaterial; 
                }
            }
        }
    }
}