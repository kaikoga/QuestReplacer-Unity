using System;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerDatabaseExtensions
    {
        public static bool HasPlatformSupport(this QuestReplacerDatabase database)
        {
            switch (database.generateMode)
            {
                case QuestReplacerDatabase.GenerateMode.Quest:
                    return Shaders.VrcMobileStandardLite && Shaders.VrcMobileToonLit;
                case QuestReplacerDatabase.GenerateMode.VRM0:
                    return Shaders.VrmMToon;
                case QuestReplacerDatabase.GenerateMode.VRM1:
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
            switch (database.generateMode)
            {
                case QuestReplacerDatabase.GenerateMode.Quest:
                    return "-q";
                case QuestReplacerDatabase.GenerateMode.VRM0:
                    return "-m";
                case QuestReplacerDatabase.GenerateMode.VRM1:
                    return "-m1";
                default:
                    throw new ArgumentOutOfRangeException(nameof(database.generateMode), database.generateMode, null);
            }
        }
    }
}