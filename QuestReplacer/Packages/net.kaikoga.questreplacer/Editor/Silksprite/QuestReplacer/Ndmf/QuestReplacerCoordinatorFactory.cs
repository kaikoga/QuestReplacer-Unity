using UnityEngine;

#if QUEST_REPLACER_VRCSDK3_AVATARS
using VRC.SDK3.Avatars.Components;
#endif

namespace Silksprite.QuestReplacer.Ndmf
{
    public static class QuestReplacerCoordinatorFactory
    {
        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform)
        {
#if QUEST_REPLACER_VRCSDK3_AVATARS
            var avatarDescriptor = avatarRootTransform.GetComponent<VRCAvatarDescriptor>();
#endif
            return new QuestReplacerCoordinator(avatarRootTransform,
                avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true));
        }
    }
}