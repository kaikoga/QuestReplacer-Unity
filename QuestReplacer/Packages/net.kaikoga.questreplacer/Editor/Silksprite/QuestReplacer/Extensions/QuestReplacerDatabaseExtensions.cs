using System;
using System.Collections.Generic;
using System.Linq;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerDatabaseExtensions
    {
        public static bool HasGenerateModeSupport(this QuestReplacerDatabase database)
        {
            switch (database.generateMode)
            {
                case QuestReplacerDatabase.GenerateMode.VRChatToonLit:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonLit;
                case QuestReplacerDatabase.GenerateMode.VRChatToonStandard:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonStandard;
                case QuestReplacerDatabase.GenerateMode.VRChatToonStandardOutline:
                    return false;
                case QuestReplacerDatabase.GenerateMode.MToon:
                    return Shaders.VrmMToon;
                case QuestReplacerDatabase.GenerateMode.MToon10:
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
                case QuestReplacerDatabase.GenerateMode.VRChatToonLit:
                    processors = MaterialDuplicator.VRChatToonLitMaterialProcessors();
                    break;
                case QuestReplacerDatabase.GenerateMode.VRChatToonStandard:
                    processors = MaterialDuplicator.VRChatToonStandardMaterialProcessors();
                    break;
                case QuestReplacerDatabase.GenerateMode.MToon:
                    processors = MaterialDuplicator.MToonMaterialProcessors(true);
                    break;
                case QuestReplacerDatabase.GenerateMode.MToon10:
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
                case QuestReplacerDatabase.Platform.VRChatMobile:
                    return "-m";
                case QuestReplacerDatabase.Platform.VRM0:
                    return "-vrm0";
                case QuestReplacerDatabase.Platform.VRM1:
                    return "-vrm1";
                case QuestReplacerDatabase.Platform.VRChatAndroid:
                    return "-a";
                case QuestReplacerDatabase.Platform.VRChatIos:
                    return "-i";
                default:
                    throw new ArgumentOutOfRangeException(nameof(database.platform), database.platform, null);
            }
        }
    }
}