using System;
using System.Collections.Generic;
using System.Linq;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerDatabaseExtensions
    {
        public static bool HasPlatformSupport(this QuestReplacerDatabase database)
        {
            switch (database.platform)
            {
                case QuestReplacerDatabase.Platform.VRChatMobile:
                case QuestReplacerDatabase.Platform.VRChatAndroid:
                case QuestReplacerDatabase.Platform.VRChatIos:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonLit;
                case QuestReplacerDatabase.Platform.VRM0:
                    return Shaders.VrmMToon;
                case QuestReplacerDatabase.Platform.VRM1:
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
                case QuestReplacerDatabase.GenerateMode.MToon:
                    processors = MaterialDuplicator.MToonMaterialProcessors(true);
                    break;
                case QuestReplacerDatabase.GenerateMode.MToon10:
                    processors = MaterialDuplicator.MToon10MaterialProcessors(true);
                    break;
                case QuestReplacerDatabase.GenerateMode.VRChatToonStandard:
                    processors = MaterialDuplicator.VRChatToonStandardMaterialProcessors();
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