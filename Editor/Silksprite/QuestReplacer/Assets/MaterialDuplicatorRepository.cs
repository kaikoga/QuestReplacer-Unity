using System.Collections.Generic;
using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class MaterialDuplicatorRepository : AssetDuplicatorRepositoryBase<Material, QuestReplacerMaterialGenerationMode>
    {
        public static readonly MaterialDuplicatorRepository Instance = new MaterialDuplicatorRepository();

        static Dictionary<QuestReplacerMaterialGenerationMode, ISingleAssetDuplicator<Material>[]> Builtins =>
            new Dictionary<QuestReplacerMaterialGenerationMode, ISingleAssetDuplicator<Material>[]>
            {
                [QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit] = new ISingleAssetDuplicator<Material>[]
                {
                    new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                    new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit),
                },
                [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard] = new ISingleAssetDuplicator<Material>[]
                {
                    new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                    new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard),
                },
                [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline] = new ISingleAssetDuplicator<Material>[]
                {
                    new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                    new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandardOutline),
                },
                [QuestReplacerMaterialGenerationMode.GenerateMToon] = new ISingleAssetDuplicator<Material>[]
                {
                    new SingleMaterialDuplicator("Standard", Shaders.Standard),
                    new SingleMaterialDuplicator("", Shaders.VrmMToon),
                },
                [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new ISingleAssetDuplicator<Material>[]
                {
                    new SingleMaterialDuplicator("Standard", Shaders.Standard),
                    new SingleMaterialDuplicator("", Shaders.VrmMToon10),
                },
            };

        MaterialDuplicatorRepository()
        {
            foreach (var (mode, builtins) in Builtins)
            {
                foreach (var builtin in builtins)
                {
                    Register(mode, builtin);
                }
            }
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonLitMaterialProcessors()
        {
            return Query(QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit);
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.ExtConvertMToon)) yield return duplicator;
            foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.GenerateMToon)) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.ExtConvertMToon10)) yield return duplicator;
            foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.GenerateMToon10)) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard)) yield return duplicator; 
            foreach (var duplicator in Query(QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard)) yield return duplicator;
        }

        public IEnumerable<ISingleAssetDuplicator<Material>> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Query(QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline);
        }
    }
}