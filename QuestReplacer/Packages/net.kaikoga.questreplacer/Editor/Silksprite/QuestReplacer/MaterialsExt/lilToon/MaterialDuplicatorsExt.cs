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
                QuestReplacerGenerateMode.ExtConvertMToon,
                new lilToonToVRMMaterialDuplicator());
            MaterialDuplicator.RegisterExt(
                QuestReplacerGenerateMode.ExtConvertMToon10,
                new lilToonToVRMMaterialDuplicatorMToon10());
        }
    }
}
