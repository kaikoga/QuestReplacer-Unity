using System.Linq;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class GenerateAnimationClipsCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Generate AnimationClips";

        public GenerateAnimationClipsCommand(QuestReplacer questReplacer, QuestReplacerContext context) : base(questReplacer, context)
        {
        }

        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var duplicator = EnsureDatabase(false)
                .CreateAnimationClipAssetDuplicator(QuestReplacerAnimationClipGenerationMode.Instantiate);
            foreach (var pair in questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is AnimationClip leftAnimationClip)
                {
                    var rightMaterial = duplicator.Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate AnimationClips");
                    pair.right = rightMaterial; 
                }
            }
        }
    }
}