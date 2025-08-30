using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Context;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerExtensions
    {
        public static QuestReplacerContext ToContext(this QuestReplacer questReplacer)
        {
            return new QuestReplacerContext(
                questReplacer.Targets,
                Enumerable.Empty<AnimatorController>(),
                questReplacer.database != null ? questReplacer.database.componentFilters : null,
                questReplacer.pairs);
        }

        public static QuestReplacerContext ToAvatarContext(this QuestReplacer questReplacer, Transform avatarRootTransform, IEnumerable<AnimatorController> animatorControllers)
        {
            return new QuestReplacerContext(
                avatarRootTransform ? new [] { avatarRootTransform } : Array.Empty<Transform>(),
                animatorControllers,
                questReplacer.database != null ? questReplacer.database.componentFilters : null,
                questReplacer.pairs);
        }

        public static QuestReplacerDatabase EnsureDatabase(this QuestReplacer questReplacer, QuestReplacerPlatform? platform)
        {
            if (questReplacer.database) return questReplacer.database;

            var databases = AssetDatabase.FindAssets($"t:{nameof(QuestReplacerDatabase)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<QuestReplacerDatabase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            QuestReplacerDatabase database;
            QuestReplacerDatabase GetPlatform(QuestReplacerPlatform value)
            {
                return database = databases.FirstOrDefault(db => db.config.platform == value);
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
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatMobile, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
                    break;
                case QuestReplacerPlatform.VRM0:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRM0, QuestReplacerMaterialGenerationMode.GenerateMToon);
                    break;
                case QuestReplacerPlatform.VRM1:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRM1, QuestReplacerMaterialGenerationMode.GenerateMToon10);
                    break;
                case QuestReplacerPlatform.VRChatAndroid:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatAndroid, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
                    break;
                case QuestReplacerPlatform.VRChatIos:
                    questReplacer.CreateDatabase(QuestReplacerPlatform.VRChatIos, QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
            return questReplacer.database;
        }

        public static void CreateDatabase(this QuestReplacer questReplacer, QuestReplacerPlatform platform, QuestReplacerMaterialGenerationMode materialGenerationMode)
        {
            Undo.RecordObject(questReplacer, "Quest Replacer: Create Database");
            var database = ScriptableObject.CreateInstance<QuestReplacerDatabase>();
            database.config.platform = platform;
            database.config.materialGenerationMode = materialGenerationMode;
            database.generatedFileSuffix = database.GetDefaultSuffix();

            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{platform} Replacer Database.asset");
            AssetDatabase.CreateAsset(database, assetPath);
            questReplacer.database = database;
            Undo.RegisterCreatedObjectUndo(database, "Quest Replacer: Create Database");
        }
    }
}