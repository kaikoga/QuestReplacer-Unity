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
                    processors = MaterialDuplicatorRepository.Instance.VRChatToonLitMaterialProcessors();
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard:
                    processors = MaterialDuplicatorRepository.Instance.VRChatToonStandardMaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline:
                    processors = MaterialDuplicatorRepository.Instance.VRChatToonStandardOutlineMaterialProcessors();
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateMToon:
                    processors = MaterialDuplicatorRepository.Instance.MToonMaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.GenerateMToon10:
                    processors = MaterialDuplicatorRepository.Instance.MToon10MaterialProcessors(false);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard:
                    processors = MaterialDuplicatorRepository.Instance.VRChatToonStandardMaterialProcessors(true);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon:
                    processors = MaterialDuplicatorRepository.Instance.MToonMaterialProcessors(true);
                    break;
                case QuestReplacerMaterialGenerationMode.ExtConvertMToon10:
                    processors = MaterialDuplicatorRepository.Instance.MToon10MaterialProcessors(true);
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

        public static AssetDuplicator<AnimationClip> CreateAnimationClipAssetDuplicator(this QuestReplacerDatabase database, QuestReplacerAnimationClipGenerationMode animationClipGenerationMode)
        {
            IEnumerable<ISingleAssetDuplicator<AnimationClip>> processors;
            switch (animationClipGenerationMode)
            {
                case QuestReplacerAnimationClipGenerationMode.Instantiate:
                    processors = AnimationClipDuplicatorRepository.Instance.AnimationClipProcessors();
                    break;
                default:
                    processors = Enumerable.Empty<ISingleAssetDuplicator<AnimationClip>>();
                    break;
            }
            return new AssetDuplicator<AnimationClip>(database.generatedDirectory,
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