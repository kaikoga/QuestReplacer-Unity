using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.MaterialsExt.VRMShaders;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicators.RegisterExt(
                QuestReplacerMaterialGenerationMode.GenerateMToon10,
                new MToonUpgrader());
        }
    }
}
