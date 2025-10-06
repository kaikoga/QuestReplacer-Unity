using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class GenerateSingleCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Generate Single";

        readonly int _index;

        public GenerateSingleCommand(QuestReplacer questReplacer, int index) : base(questReplacer)
        {
            _index = index;
        }

        protected override void DoExecute()
        {
            var pair = QuestReplacer.pairs[_index];

            switch (pair.left)
            {
                case Material leftMaterial: 
                {
                    var rightMaterial = GetMaterialAssetDuplicator().Duplicate(leftMaterial);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, Name);
                    pair.right = rightMaterial;
                    break;
                }
                case AnimationClip leftAnimationClip:
                {
                    var rightMaterial = GetAnimationClipAssetDuplicator().Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, Name);
                    pair.right = rightMaterial; 
                    break;
                }
            }
        }
    }
}
