using System;
using System.Linq;
using UnityEngine;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerExtensions
    {
        public static QuestReplacerContext ToContext(this QuestReplacer questReplacer)
        {
            return new QuestReplacerContext(
                questReplacer.Targets,
                questReplacer.database != null ? questReplacer.database.componentFilters : null,
                questReplacer.pairs);
        }

        public static QuestReplacerReusableContext ToReusableContext(this QuestReplacer questReplacer)
        {
            return new QuestReplacerReusableContext(questReplacer);
        }

        public static QuestReplacerDatabase EnsureDatabase(this QuestReplacer questReplacer, QuestReplacerPlatform? platform)
        {
            if (questReplacer.database) return questReplacer.database;

            var databases = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(QuestReplacerDatabase)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<QuestReplacerDatabase>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            QuestReplacerDatabase database;
            QuestReplacerDatabase GetPlatform(QuestReplacerPlatform value)
            {
                return database = databases.FirstOrDefault(db => db.platform == value);
            }

            if (platform is QuestReplacerPlatform value)
            {
                switch (value)
                {
                    case QuestReplacerPlatform.VRChatAndroid:
                    case QuestReplacerPlatform.VRChatIos:
                        var _ = GetPlatform(value) || GetPlatform(QuestReplacerPlatform.VRChatMobile);
                        break;
                    default:
                        GetPlatform(value);
                        break;
                }
            }
            else
            {
                database = databases.FirstOrDefault();
            }

            questReplacer.database = database;
            if (database) return database;

            switch (platform)
            {
                case null:
                case QuestReplacerPlatform.VRChatMobile:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerGenerateMode.GenerateVRChatToonStandard);
                    break;
                case QuestReplacerPlatform.VRM0:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRM0, QuestReplacerGenerateMode.GenerateMToon);
                    break;
                case QuestReplacerPlatform.VRM1:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRM1, QuestReplacerGenerateMode.GenerateMToon10);
                    break;
                case QuestReplacerPlatform.VRChatAndroid:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatAndroid, QuestReplacerGenerateMode.GenerateVRChatToonStandard);
                    break;
                case QuestReplacerPlatform.VRChatIos:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatIos, QuestReplacerGenerateMode.GenerateVRChatToonStandard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
            return questReplacer.database;
        }

        public static void CreateDatabase(this QuestReplacer questReplacer, QuestReplacerPlatform platform, QuestReplacerGenerateMode generateMode)
        {
            var database = ScriptableObject.CreateInstance<QuestReplacerDatabase>();
            database.platform = platform;
            database.generateMode = generateMode;
            database.generatedFileSuffix = database.GetDefaultSuffix();

            var assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/{platform} Replacer Database.asset");
            UnityEditor.AssetDatabase.CreateAsset(database, assetPath);
            questReplacer.database = database;
        }
    }
}