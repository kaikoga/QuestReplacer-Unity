using Silksprite.QuestReplacer.Assets;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicatorRepository.Instance.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon,
                new lilToonToVRMMaterialDuplicator());
            MaterialDuplicatorRepository.Instance.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertMToon10,
                new lilToonToVRMMaterialDuplicatorMToon10());
        }
    }
}
