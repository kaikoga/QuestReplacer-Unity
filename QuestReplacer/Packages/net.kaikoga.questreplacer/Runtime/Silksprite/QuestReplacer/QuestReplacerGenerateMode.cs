using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public enum QuestReplacerGenerateMode
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
    }
}
