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
            
            var database = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(QuestReplacerDatabase)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<QuestReplacerDatabase>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                // ReSharper disable once SimplifyConditionalTernaryExpression
                .FirstOrDefault(db => platform is QuestReplacerDatabase.Platform value ? db.platform == value : true);
            questReplacer.database = database;
            if (database) return database;

            switch (platform)
            {
                case null:
                case QuestReplacerDatabase.Platform.Quest:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.Quest, QuestReplacerDatabase.GenerateMode.Quest);
                    break;
                case QuestReplacerDatabase.Platform.VRM0:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.Quest, QuestReplacerDatabase.GenerateMode.VRM0);
                    break;
                case QuestReplacerDatabase.Platform.VRM1:
                    questReplacer.CreateDatabase(QuestReplacerDatabase.Platform.VRM1, QuestReplacerDatabase.GenerateMode.VRM1);
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