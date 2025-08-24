using Silksprite.QuestReplacer.Context;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    public static class QuestReplacerCoordinatorFactory
    {
        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform)
        {
            return new QuestReplacerCoordinator(avatarRootTransform,
                AnimatorControllerExtractor.ExtractFrom(avatarRootTransform),
                avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true));
        }
    }
}