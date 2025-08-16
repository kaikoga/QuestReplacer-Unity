using Silksprite.QuestReplacer.Materials;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicator.RegisterExt(
                QuestReplacerGenerateMode.ExtConvertMToon,
                new lilToonToVRMMaterialDuplicator(false));
            MaterialDuplicator.RegisterExt(
                QuestReplacerGenerateMode.ExtConvertMToon10,
                new lilToonToVRMMaterialDuplicator(true));
            MaterialDuplicator.RegisterExt(
                QuestReplacerGenerateMode.ExtConvertVRChatToonStandard,
                new VRCQuestToolsMaterialDuplicator());
        }
    }
}
