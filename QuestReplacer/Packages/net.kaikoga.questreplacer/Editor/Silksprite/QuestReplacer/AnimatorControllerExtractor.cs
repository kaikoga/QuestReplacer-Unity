using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using System.Linq;
using nadena.dev.ndmf;

#if QUESTREPLACER_VRCSDK3_AVATARS
using VRC.SDK3.Avatars.Components;
#endif

namespace Silksprite.QuestReplacer
{
    public static class AnimatorControllerExtractor
    {
        public static IEnumerable<AnimatorController> ExtractFrom(Transform avatarRootTransform, bool cloneAnimations)
        {
#if QUESTREPLACER_VRCSDK3_AVATARS
            IEnumerable<AnimatorController> ExtractFromLayers(VRCAvatarDescriptor.CustomAnimLayer[] animationLayers)
            {
                for (var i = 0; i < animationLayers.Length; i++)
                {
                    var layer = animationLayers[i];
                    if (layer.animatorController)
                    {
                        var animatorController = (AnimatorController) layer.animatorController;
#if QUESTREPLACER_ADLIB
                        if (cloneAnimations)
                        {
                            var (mainAsset, mappings) = new AdLib.Utils.CustomCloneAnimatorController(cloneClips: true).CloneWithMappings(animatorController);
                            animatorController = mainAsset;
                            layer.animatorController = animatorController;
#if QUESTREPLACER_NDMF_SUPPORT
                            foreach (var (key, value) in mappings)
                            {
                                ObjectRegistry.ActiveRegistry.RegisterReplacedObject(key, value);
                            }
#endif
                        }
#endif
                        yield return animatorController;
                    }
                    animationLayers[i] = layer;
                }
            }

            if (avatarRootTransform.TryGetComponent(out VRCAvatarDescriptor avatarDescriptor)
                && avatarDescriptor.customizeAnimationLayers)
            {
                return ExtractFromLayers(avatarDescriptor.baseAnimationLayers)
                    .Concat(ExtractFromLayers(avatarDescriptor.specialAnimationLayers));
            }
#endif
            return Enumerable.Empty<AnimatorController>();
        }
    }
}
