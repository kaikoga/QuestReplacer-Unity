using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerExtensions
    {
        public static QuestReplacerContext Context(this QuestReplacer questReplacer)
        {
            var avatarRoot = questReplacer.avatarRoot;
            var targets = avatarRoot ? new[] { avatarRoot } : SceneManager.GetActiveScene().GetRootGameObjects().Select(gameObject => gameObject.transform);
            return new QuestReplacerContext(targets, questReplacer.pairs);
        }

        public static QuestReplacerDatabase EnsureDatabase(this QuestReplacer questReplacer, QuestReplacerDatabase.GenerateMode? generateMode)
        {
            if (questReplacer.database) return questReplacer.database;
            
            var database = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(QuestReplacerDatabase)}")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<QuestReplacerDatabase>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                // ReSharper disable once SimplifyConditionalTernaryExpression
                .FirstOrDefault(db => generateMode is QuestReplacerDatabase.GenerateMode value ? db.generateMode == value : true);
            questReplacer.database = database;
            if (database) return database;

            questReplacer.CreateDatabase(generateMode ?? QuestReplacerDatabase.GenerateMode.Quest);
            return questReplacer.database;
        }

        public static void CreateDatabase(this QuestReplacer questReplacer, QuestReplacerDatabase.GenerateMode generateMode)
        {
            var database = ScriptableObject.CreateInstance<QuestReplacerDatabase>();
            database.generateMode = generateMode;
            database.generatedFileSuffix = database.GetDefaultSuffix();

            var assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/{generateMode} Replacer Database.asset");
            UnityEditor.AssetDatabase.CreateAsset(database, assetPath);
            questReplacer.database = database;
        }
    }
}