using Silksprite.QuestReplacer.Materials;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicator.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon,
                new lilToonToVRMMaterialDuplicator());
            MaterialDuplicator.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon10,
                new lilToonToVRMMaterialDuplicatorMToon10());
        }
    }
}
