using Silksprite.QuestReplacer.Context;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    public static class QuestReplacerCoordinatorFactory
    {
        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform, bool cloneAnimations)
        {
            return new QuestReplacerCoordinator(avatarRootTransform,
                AnimatorControllerExtractor.ExtractFrom(avatarRootTransform, cloneAnimations),
                avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true));
        }
    }
}