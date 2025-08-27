using Silksprite.QuestReplacer.Materials;
using UnityEditor;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public static class MaterialDuplicatorsExt
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            MaterialDuplicator.RegisterExt(
                QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard,
                new VRCQuestToolsMaterialDuplicator());
        }
    }
}
