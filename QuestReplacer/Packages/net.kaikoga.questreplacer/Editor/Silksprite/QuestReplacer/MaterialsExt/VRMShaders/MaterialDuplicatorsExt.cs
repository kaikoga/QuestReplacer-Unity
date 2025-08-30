using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.MaterialsExt.VRMShaders;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicatorRepository.Instance.RegisterExt(
                QuestReplacerMaterialGenerationMode.GenerateMToon10,
                new MToonUpgrader());
        }
    }
}
