using System.Linq;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class GenerateAnimationClipsCommand
    {
        public static void DoGenerateAnimationClips(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            var duplicator = questReplacer
                .EnsureDatabase(null)
                .CreateAnimationClipAssetDuplicator(QuestReplacerAnimationClipGenerationMode.Instantiate);
            Undo.SetCurrentGroupName("QuestReplacer: Generate AnimationClips");
            Undo.RecordObject(questReplacer, "QuestReplacer: Generate AnimationClips");
            foreach (var pair in questReplacer.pairs.Where(pair => pair.LikelyUnset))
            {
                if (pair.left is AnimationClip leftAnimationClip)
                {
                    var rightMaterial = duplicator.Duplicate(leftAnimationClip);
                    Undo.RegisterCreatedObjectUndo(rightMaterial, "QuestReplacer: Generate AnimationClips");
                    pair.right = rightMaterial; 
                }
            }
            CommandBase.UpdateTypeFilters(questReplacer, context);
        }
    }
}