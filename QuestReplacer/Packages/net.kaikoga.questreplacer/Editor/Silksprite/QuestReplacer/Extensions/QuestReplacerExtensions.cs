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

        public static QuestReplacerDatabase EnsureDatabase(this QuestReplacer questReplacer, QuestReplacerDatabase.Platform? platform)
        {
            if (questReplacer.database) return questReplacer.database;

            var databases = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(QuestReplacerDatabase)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<QuestReplacerDatabase>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            QuestReplacerDatabase database;
            QuestReplacerDatabase GetPlatform(QuestReplacerDatabase.Platform value)
            {
                return database = databases.FirstOrDefault(db => db.platform == value);
            }

            if (platform is QuestReplacerDatabase.Platform value)
            {
                switch (value)
                {
                    case QuestReplacerDatabase.Platform.VRChatAndroid:
                    case QuestReplacerDatabase.Platform.VRChatIos:
                        var _ = GetPlatform(value) || GetPlatform(QuestReplacerDatabase.Platform.VRChatMobile);
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
                case QuestReplacerDatabase.Platform.VRChatMobile:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRChatMobile, QuestReplacerDatabase.GenerateMode.VRChatToonStandard);
                    break;
                case QuestReplacerDatabase.Platform.VRM0:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRM0, QuestReplacerDatabase.GenerateMode.VRM0);
                    break;
                case QuestReplacerDatabase.Platform.VRM1:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRM1, QuestReplacerDatabase.GenerateMode.VRM1);
                    break;
                case QuestReplacerDatabase.Platform.VRChatAndroid:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRChatAndroid, QuestReplacerDatabase.GenerateMode.VRChatToonStandard);
                    break;
                case QuestReplacerDatabase.Platform.VRChatIos:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRChatIos, QuestReplacerDatabase.GenerateMode.VRChatToonStandard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
            return questReplacer.database;
        }

        public static void CreateDatabase(this QuestReplacer questReplacer, QuestReplacerDatabase.Platform platform, QuestReplacerDatabase.GenerateMode generateMode)
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