using System.Collections.Generic;
using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class MaterialDuplicatorRepository : AssetDuplicatorRepositoryBase<Material>
    {
        public static readonly MaterialDuplicatorRepository Instance = new();

        readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>> Builtins = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<Material>>>
        {
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleAssetDuplicatorComparer<Material>.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonLit)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleAssetDuplicatorComparer<Material>.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonStandard)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleAssetDuplicatorComparer<Material>.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleAssetDuplicator("", Shaders.VrcMobileToonStandardOutline)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleAssetDuplicatorComparer<Material>.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.Standard),
                new SingleAssetDuplicator("", Shaders.VrmMToon)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new SortedSet<ISingleAssetDuplicator<Material>>(SingleAssetDuplicatorComparer<Material>.Instance)
            {
                new SingleAssetDuplicator("Standard", Shaders.Standard),
                new SingleAssetDuplicator("", Shaders.VrmMToon10)
            },
        };

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonLitMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit];
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon]) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon10]) yield return duplicator; 
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.GenerateMToon10]) yield return duplicator;
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon10]) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard]) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline];
        }
    }
}