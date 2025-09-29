using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public static class QuestReplacerMenuItems
    {
        [MenuItem("GameObject/QuestReplacer/New VRChat Mobile", false, 40)]
        static void CreateQuestReplacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerPlatform.VRChatMobile);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM0", false, 40)]
        static void CreateVRM0Replacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerPlatform.VRM0);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM1", false, 40)]
        static void CreateVRM1Replacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerPlatform.VRM1);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM0", true, 40)]
        [MenuItem("GameObject/QuestReplacer/New VRM1", true, 40)]
        static bool CheckVrmSupport(MenuCommand _)
        {
            return Shader.Find("VRM/MToon");
        }

        static void CreateQuestReplacer(QuestReplacerPlatform platform)
        {
            var avatarRoot = Selection.activeGameObject;

#if QUESTREPLACER_NDMF_SUPPORT
            if (avatarRoot)
            {
                CreateQuestReplacerNdmf(avatarRoot, platform);
                return;
            }
#endif
            CreateQuestReplacerNonNdmf(avatarRoot, platform);
        }
        
        static void CreateQuestReplacerNdmf(GameObject avatarRoot, QuestReplacerPlatform platform)
        {
            var gameObject = new GameObject($"QuestReplacer {platform}");
            gameObject.transform.SetParent(avatarRoot.transform, false);
            var config = gameObject.AddComponent<QuestReplacer>();
            config.EnsureDatabase(platform);
        }

        static void CreateQuestReplacerNonNdmf(GameObject avatarRoot, QuestReplacerPlatform platform)
        {
            var gameObject = new GameObject(avatarRoot ? $"QuestReplacer_{avatarRoot.name} {platform}" : $"QuestReplacer {platform}");
            var config = gameObject.AddComponent<QuestReplacer>();
            config.Target = avatarRoot ? avatarRoot.transform : null;
            config.EnsureDatabase(platform);
        }
    }
}