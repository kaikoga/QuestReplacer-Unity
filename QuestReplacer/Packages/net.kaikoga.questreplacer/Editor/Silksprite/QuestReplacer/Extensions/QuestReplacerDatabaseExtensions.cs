using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Materials;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerDatabaseExtensions
    {
        public static bool HasGenerateModeSupport(this QuestReplacerDatabase database)
        {
            switch (database.generateMode)
            {
                case QuestReplacerGenerateMode.GenerateVRChatToonLit:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonLit;
                case QuestReplacerGenerateMode.GenerateVRChatToonStandard:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonStandard;
                case QuestReplacerGenerateMode.GenerateVRChatToonStandardOutline:
                    return false;
                case QuestReplacerGenerateMode.GenerateMToon:
                case QuestReplacerGenerateMode.ExtConvertMToon:
                    return Shaders.VrmMToon;
                case QuestReplacerGenerateMode.GenerateMToon10:
                case QuestReplacerGenerateMode.ExtConvertMToon10:
                    return Shaders.VrmMToon && Shaders.VrmMToon10;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static MaterialDuplicator CreateMaterialDuplicator(this QuestReplacerDatabase database)
        {
            IEnumerable<ISingleMaterialDuplicator> processors;
            switch (database.generateMode)
            {
                case QuestReplacerGenerateMode.GenerateVRChatToonLit:
                    processors = MaterialDuplicator.VRChatToonLitMaterialProcessors();
                    break;
                case QuestReplacerGenerateMode.GenerateVRChatToonStandard:
                    processors = MaterialDuplicator.VRChatToonStandardMaterialProcessors();
                    break;
                case QuestReplacerGenerateMode.GenerateVRChatToonStandardOutline:
                    processors = MaterialDuplicator.VRChatToonStandardOutlineMaterialProcessors();
                    break;
                case QuestReplacerGenerateMode.GenerateMToon:
                    processors = MaterialDuplicator.MToonMaterialProcessors(false);
                    break;
                case QuestReplacerGenerateMode.GenerateMToon10:
                    processors = MaterialDuplicator.MToon10MaterialProcessors(false);
                    break;
                case QuestReplacerGenerateMode.ExtConvertMToon:
                    processors = MaterialDuplicator.MToonMaterialProcessors(true);
                    break;
                case QuestReplacerGenerateMode.ExtConvertMToon10:
                    processors = MaterialDuplicator.MToon10MaterialProcessors(true);
                    break;
                default:
                    processors = Enumerable.Empty<ISingleMaterialDuplicator>();
                    break;
            }
            return new MaterialDuplicator(database.generatedDirectory,
                database.generatedFilePrefix,
                database.generatedFileSuffix,
                processors.ToArray());
        }

        public static string GetDefaultSuffix(this QuestReplacerDatabase database)
        {
            switch (database.platform)
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
                    throw new ArgumentOutOfRangeException(nameof(database.platform), database.platform, null);
            }
        }
    }
}