using System.Collections;
using System.Collections.Generic;
using Silksprite.QuestReplacer.Materials.VRMShaders;

namespace Silksprite.QuestReplacer.Materials
{
    public static class MaterialDuplicators
    {
        public static bool RegisterExt(QuestReplacerMaterialGenerationMode materialGenerationMode, ISingleMaterialDuplicator duplicator)
        {
            if (!Exts.TryGetValue(materialGenerationMode, out var list))
            {
                return false;
            }
            list.Add(duplicator);
            return true;
        }

        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>> Builtins = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>>
        {
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandardOutline)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new MToonUpgrader(),
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon10)
            },
        };
        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>> Exts = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>>
        {
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon10] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
        };
        class SingleMaterialDuplicatorComparer : IComparer<ISingleMaterialDuplicator>
        {
            public static SingleMaterialDuplicatorComparer Instance => new SingleMaterialDuplicatorComparer();

            public int Compare(ISingleMaterialDuplicator x, ISingleMaterialDuplicator y)
            {
                return Comparer.Default.Compare(y?.Priority, x?.Priority);
            }
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonLitMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit];
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon10]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon10]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline];
        }
    }
}