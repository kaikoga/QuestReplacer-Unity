using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

#if QUESTREPLACER_VRCSDK3_AVATARS
using System.Linq;
using VRC.SDK3.Avatars.Components;
#endif

namespace Silksprite.QuestReplacer
{
    public static class AnimatorControllerExtractor
    {
        public static IEnumerable<AnimatorController> ExtractFrom(Transform avatarRootTransform)
        {
#if QUESTREPLACER_VRCSDK3_AVATARS
            if (avatarRootTransform.TryGetComponent(out VRCAvatarDescriptor avatarDescriptor)
                && avatarDescriptor.customizeAnimationLayers)
            {
                return avatarDescriptor.baseAnimationLayers
                    .Concat(avatarDescriptor.specialAnimationLayers)
                    .Where(layer => !layer.isDefault)
                    .Select(layer => layer.animatorController)
                    .Where(controller => controller)
                    .Cast<AnimatorController>();
            }
#endif
            return Enumerable.Empty<AnimatorController>();
        }
    }
}
