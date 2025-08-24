using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    static class QuestReplacerPlatformDetector
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
            if (IsVrm0(avatarRootTransform))
            {
                platform = QuestReplacerBuildPlatform.VRM0;
            }
            else if (IsVrm1(avatarRootTransform))
            {
                platform = QuestReplacerBuildPlatform.VRM1;
            }
            else if (IsVRChat(avatarRootTransform))
            {
                if (IsStandalone())
                {
                    platform = QuestReplacerBuildPlatform.VRChatPC;
                }
                if (IsAndroid())
                {
                    platform = QuestReplacerBuildPlatform.VRChatAndroid;
                }
                else if (IsIos())
                {
                    platform = QuestReplacerBuildPlatform.VRChatIos;
                }
                else
                {
                    platform = default;
                    return false;
                }
            }
            else
            {
                platform = QuestReplacerBuildPlatform.Generic;
            }
            return true;
        }
    }
}
