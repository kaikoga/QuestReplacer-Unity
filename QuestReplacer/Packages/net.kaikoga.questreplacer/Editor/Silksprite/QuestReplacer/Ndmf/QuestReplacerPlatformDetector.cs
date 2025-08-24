using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    static class QuestReplacerPlatformDetector
    {

#if QUEST_REPLACER_VRM0
        static bool IsVrm0(GameObject avatarRootObject) => avatarRootObject.GetComponent<VRM.VRMMeta>();
#else
        static bool IsVrm0(GameObject avatarRootObject) => false;
#endif

#if QUEST_REPLACER_VRM1
        static bool IsVrm1(GameObject avatarRootObject) => avatarRootObject.GetComponent<UniVRM10.Vrm10Instance>();
#else
        static bool IsVrm1(GameObject avatarRootObject) => false;
#endif

#if QUEST_REPLACER_VRCSDK3_AVATARS
        static bool IsVRChat(GameObject avatarRootObject) => avatarRootObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
#else
        static bool IsVRChat(GameObject avatarRootObject) => false;
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

        public static bool TryGetPlatformForAvatar(GameObject avatarRootObject, out QuestReplacerBuildPlatform platform)
        {
            if (IsVrm0(avatarRootObject))
            {
                platform = QuestReplacerBuildPlatform.VRM0;
            }
            else if (IsVrm1(avatarRootObject))
            {
                platform = QuestReplacerBuildPlatform.VRM1;
            }
            else if (IsVRChat(avatarRootObject))
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
