using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerDatabaseExtensions
    {
        public static bool HasGenerateModeSupport(this QuestReplacerDatabase database)
        {
            switch (database.config.materialGenerationMode)
            {
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonLit;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard:
                case QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonStandard;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline:
                    return false;
                case QuestReplacerMaterialGenerationMode.GenerateMToon:
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon:
                    return Shaders.VrmMToon;
                case QuestReplacerMaterialGenerationMode.GenerateMToon10:
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon10:
                    return Shaders.VrmMToon && Shaders.VrmMToon10;
                default:
                    return false;
            }
        }

        public static AssetDuplicator<Material> CreateMaterialAssetDuplicator(this QuestReplacerDatabase database, QuestReplacerMaterialGenerationMode materialGenerationMode)
        {
            IEnumerable<ISingleAssetDuplicator<Material>> processors;
            switch (materialGenerationMode)
            {
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit:
                    processors = MaterialDuplicators.VRChatToonLitMaterialProcessors();
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard:
                    processors = MaterialDuplicators.VRChatToonStandardMaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline:
                    processors = MaterialDuplicators.VRChatToonStandardOutlineMaterialProcessors();
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateMToon:
                    processors = MaterialDuplicators.MToonMaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateMToon10:
                    processors = MaterialDuplicators.MToon10MaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard:
                    processors = MaterialDuplicators.VRChatToonStandardMaterialProcessors(true);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon:
                    processors = MaterialDuplicators.MToonMaterialProcessors(true);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon10:
                    processors = MaterialDuplicators.MToon10MaterialProcessors(true);
                    break;
                default:
                    processors = Enumerable.Empty<ISingleAssetDuplicator<Material>>();
                    break;
            }
            return new AssetDuplicator<Material>(database.generatedDirectory,
                database.generatedFilePrefix,
                database.generatedFileSuffix,
                processors.ToArray());
        }

        public static string GetDefaultSuffix(this QuestReplacerDatabase database)
        {
            var platform = database.config.platform;
            switch (platform)
            {
                case QuestReplacerPlatform.VRChatMobile:
                    return "-m";
                case QuestReplacerPlatform.VRM0:
                    return "-vrm0";
                case QuestReplacerPlatform.VRM1:
                    return "-vrm1";
                case QuestReplacerPlatform.VRChatAndroid:
                    return "-a";
                case QuestReplacerPlatform.VRChatIos:
                    return "-i";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}