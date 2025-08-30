using Silksprite.QuestReplacer.Assets;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicators.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard,
                new VRCQuestToolsMaterialDuplicator());
        }
    }
}
