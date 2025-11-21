using Silksprite.QuestReplacer.Context;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ablet
{
    public static class QuestReplacerCoordinatorFactory
    {
        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform, bool cloneAnimations)
        {
            return new QuestReplacerCoordinator(
                avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true),
                cloneAnimations);
        }
    }
}