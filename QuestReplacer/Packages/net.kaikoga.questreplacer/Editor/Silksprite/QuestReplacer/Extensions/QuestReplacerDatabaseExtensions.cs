using System;

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
            ISingleMaterialDuplicator[] processors = null;
            switch (database.generateMode)
            {
                case QuestReplacerDatabase.GenerateMode.Quest:
                    processors = MaterialDuplicator.QuestMaterialProcessors;
                    break;
                case QuestReplacerDatabase.GenerateMode.VRM0:
                    processors = MaterialDuplicator.VRM0MaterialProcessors;
                    break;
                case QuestReplacerDatabase.GenerateMode.VRM1:
                    processors = MaterialDuplicator.VRM1MaterialProcessors;
                    break;
            }
            return new MaterialDuplicator(database.generatedDirectory,
                database.generatedFilePrefix,
                database.generatedFileSuffix,
                processors);
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