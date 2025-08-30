using System.Collections;
using System.Collections.Generic;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Materials.VRMShaders;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public static class MaterialDuplicators
    {
        public static bool RegisterExt(QuestReplacerMaterialGenerationMode materialGenerationMode, ISingleAssetDuplicator<Material> duplicator)
        {
            if (!Exts.TryGetValue(materialGenerationMode, out var list))
            {
                return false;
            }
            list.Add(duplicator);
            return true;
        }

        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>> Builtins = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>>
        {
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonLit)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonStandard)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonStandardOutline)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.Standard),
                new SingleAssetDuplicator("", Shaders.VrmMToon)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance)
            {
                new MToonUpgrader(),
                new SingleAssetDuplicator("Standard", Shaders.Standard),
                new SingleAssetDuplicator("", Shaders.VrmMToon10)
            },
        };
        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>> Exts = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>>
        {
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon10] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleMaterialDuplicatorComparer.Instance),
        };
        class SingleMaterialDuplicatorComparer : IComparer<ISingleAssetDuplicator<Material>>
        {
            public static SingleMaterialDuplicatorComparer Instance => new SingleMaterialDuplicatorComparer();

            public int Compare(ISingleAssetDuplicator<Material> x, ISingleAssetDuplicator<Material> y)
            {
                return Comparer.Default.Compare(y?.Priority, x?.Priority);
            }
        }

        public static IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonLitMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit];
        }

        public static IEnumerable<ISingleAssetDuplicator<Material>> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon]) yield return duplicator;
        }

        public static IEnumerable<ISingleAssetDuplicator<Material>> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon10]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon10]) yield return duplicator;
        }

        public static IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard]) yield return duplicator;
        }

        public static IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline];
        }
    }
}