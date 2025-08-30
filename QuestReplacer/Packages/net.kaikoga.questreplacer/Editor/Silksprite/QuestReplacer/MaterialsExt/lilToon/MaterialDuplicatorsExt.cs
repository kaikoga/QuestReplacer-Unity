using Silksprite.QuestReplacer.Assets;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicators.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon,
                new lilToonToVRMMaterialDuplicator());
            MaterialDuplicators.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon10,
                new lilToonToVRMMaterialDuplicatorMToon10());
        }
    }
}
