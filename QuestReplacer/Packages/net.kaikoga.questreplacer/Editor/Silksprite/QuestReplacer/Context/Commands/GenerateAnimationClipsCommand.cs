using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class GenerateAnimationClipsCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Generate AnimationClips";

        public GenerateAnimationClipsCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            foreach (var pair in QuestReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is AnimationClip leftAnimationClip)
                {
                    var rightMaterial = GetAnimationClipAssetDuplicator().Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, Name);
                    pair.right = rightMaterial; 
                }
            }
        }
    }
}