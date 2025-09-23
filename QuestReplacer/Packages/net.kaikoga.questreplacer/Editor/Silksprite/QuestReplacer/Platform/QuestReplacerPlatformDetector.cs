using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silksprite.QuestReplacer.Platform
{
    public static class QuestReplacerPlatformDetector
    {

#if QUESTREPLACER_VRM0
        static bool IsVrm0(Transform avatarRootTransform) => avatarRootTransform.GetComponent<VRM.VRMMeta>();
#else
        static bool IsVrm0(Transform avatarRootTransform) => false;
#endif

#if QUESTREPLACER_VRM1
        static bool IsVrm1(Transform avatarRootTransform) => avatarRootTransform.GetComponent<UniVRM10.Vrm10Instance>();
#else
        static bool IsVrm1(Transform avatarRootTransform) => false;
#endif

#if QUESTREPLACER_VRCSDK3_AVATARS
        static bool IsVRChat(Transform avatarRootTransform) => avatarRootTransform.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
#else
        static bool IsVRChat(Transform avatarRootTransform) => false;
#endif

#if UNITY_STANDALONE
        static bool IsStandalone() => true;
#else
        static bool IsStandalone() => false;
#endif

#if UNITY_ANDROID
        static bool IsAndroid() => true;
#else
        static bool IsAndroid() => false;
#endif

#if UNITY_IOS
        static bool IsIos() => true;
#else
        static bool IsIos() => false;
#endif

        public static bool TryGetPlatformForAvatar(Transform avatarRootTransform, out QuestReplacerBuildPlatform platform)
        {
            if (TryGetSinglePlatformForAvatar(avatarRootTransform, out platform))
            {
                return true;
            }
            
#if QUESTREPLACER_NDMF_SUPPORT
            // only comes here if avatar platform cannot be decided
            var ndmfPlatform = nadena.dev.ndmf.platform.AmbientPlatform.CurrentPlatform;
            switch (ndmfPlatform.AvatarRootComponentType?.Name)
            {
                case nameof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor):
                    return TryGetVRChatPlatform(out platform);
                case nameof(VRM.VRMMeta):
                    platform = QuestReplacerBuildPlatform.VRM0;
                    return true;
                case nameof(UniVRM10.Vrm10Instance):
                    platform = QuestReplacerBuildPlatform.VRM1;
                    return true;
            }
#endif
            
            return false;
        }

        static bool TryGetSinglePlatformForAvatar(Transform avatarRootTransform, out QuestReplacerBuildPlatform platform)
        {
            var platforms = TryGetPlatformsForAvatar(avatarRootTransform).ToArray();
            switch (platforms.Length)
            {
                case 0:
                    platform = QuestReplacerBuildPlatform.Generic;
                    return true;
                case 1:
                    platform = platforms[0];
                    return true;
                default:
                    platform = QuestReplacerBuildPlatform.Generic;
                    return false;
            }
        }
        
        static IEnumerable<QuestReplacerBuildPlatform> TryGetPlatformsForAvatar(Transform avatarRootTransform)
        {
            if (IsVrm0(avatarRootTransform))
            {
                yield return QuestReplacerBuildPlatform.VRM0;
            }
            if (IsVrm1(avatarRootTransform))
            {
                yield return QuestReplacerBuildPlatform.VRM1;
            }
            if (IsVRChat(avatarRootTransform))
            {
                if (TryGetVRChatPlatform(out var vrcPlatform))
                {
                    yield return vrcPlatform;
                }
            }
        }

        static bool TryGetVRChatPlatform(out QuestReplacerBuildPlatform vrcPlatform)
        {
            if (IsStandalone())
            {
                vrcPlatform = QuestReplacerBuildPlatform.VRChatPC;
                return true;
            }
            if (IsAndroid())
            {
                vrcPlatform = QuestReplacerBuildPlatform.VRChatAndroid;
                return true;
            }
            if (IsIos())
            {
                vrcPlatform = QuestReplacerBuildPlatform.VRChatIos;
                return true;
            }
            vrcPlatform = default;
            return false;
        }
    }
}
