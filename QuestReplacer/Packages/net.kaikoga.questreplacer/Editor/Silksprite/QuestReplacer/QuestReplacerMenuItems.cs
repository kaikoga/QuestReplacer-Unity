using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public static class QuestReplacerMenuItems
    {
        [MenuItem("GameObject/QuestReplacer/New Quest", false, 40)]
        static void CreateQuestReplacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerDatabase.GenerateMode.Quest);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM0", false, 40)]
        static void CreateVRM0Replacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerDatabase.GenerateMode.VRM0);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM1", false, 40)]
        static void CreateVRM1Replacer(MenuCommand _)
        {
            CreateQuestReplacer(QuestReplacerDatabase.GenerateMode.VRM1);
        }

        [MenuItem("GameObject/QuestReplacer/New VRM0", true, 40)]
        [MenuItem("GameObject/QuestReplacer/New VRM1", true, 40)]
        static bool CheckVrmSupport(MenuCommand _)
        {
            return Shader.Find("VRM/MToon");
        }

        static void CreateQuestReplacer(QuestReplacerDatabase.GenerateMode generateMode)
        {
            var avatarRoot = Selection.activeGameObject;

            var gameObject = new GameObject(avatarRoot ? $"{generateMode}Replacer_{avatarRoot.name}" : $"{generateMode}Replacer");
            var config = gameObject.AddComponent<QuestReplacer>();
            config.avatarRoot = avatarRoot ? avatarRoot.transform : null;

            config.EnsureDatabase(generateMode);
        }
    }
}