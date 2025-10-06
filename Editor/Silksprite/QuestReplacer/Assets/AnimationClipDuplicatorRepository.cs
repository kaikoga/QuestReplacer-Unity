using System.Collections.Generic;
using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class AnimationClipDuplicatorRepository : AssetDuplicatorRepositoryBase<AnimationClip, QuestReplacerAnimationClipGenerationMode>
    {
        public static readonly AnimationClipDuplicatorRepository Instance = new AnimationClipDuplicatorRepository();

        static Dictionary<QuestReplacerAnimationClipGenerationMode, ISingleAssetDuplicator<AnimationClip>[]> Builtins =>
            new Dictionary<QuestReplacerAnimationClipGenerationMode, ISingleAssetDuplicator<AnimationClip>[]>
            {
                [QuestReplacerAnimationClipGenerationMode.Instantiate] = new ISingleAssetDuplicator<AnimationClip>[]
                {
                    new SingleAssetDuplicator<AnimationClip>(),
                },
            };

        AnimationClipDuplicatorRepository()
        {
            foreach (var (mode, builtins) in Builtins)
            {
                foreach (var builtin in builtins)
                {
                    Register(mode, builtin);
                }
            }
        }

        public IEnumerable<ISingleAssetDuplicator<AnimationClip>> AnimationClipProcessors()
        {
            return Query(QuestReplacerAnimationClipGenerationMode.Instantiate);
        }
    }
}