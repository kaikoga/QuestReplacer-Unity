using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public enum QuestReplacerMaterialGenerationMode
    {
        [InspectorName("Generate VRChat ToonLit")]
        GenerateVRChatToonLit = 0x110,
        [InspectorName("Generate VRChat Toon Standard")]
        GenerateVRChatToonStandard = 0x120,
        [InspectorName("Generate VRChat Toon Standard (Outline)")]
        GenerateVRChatToonStandardOutline = 0x121,
        [InspectorName("Generate MToon")]
        GenerateMToon = 0x200,
        [InspectorName("Generate MToon10")]
        GenerateMToon10 = 0x210,
        [InspectorName("Ext Convert VRChat Toon Standard")]
        ExtConvertVRChatToonStandard = 0x2120,
        [InspectorName("Ext Convert MToon")]
        ExtConvertMToon = 0x2200,
        [InspectorName("Ext Convert MToon10")]
        ExtConvertMToon10 = 0x2210,
    }
}
