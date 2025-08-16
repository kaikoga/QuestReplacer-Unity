using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public enum QuestReplacerGenerateMode
    {
        [InspectorName("Generate VRChat ToonLit")]
        GenerateVRChatToonLit = 0x00,
        [InspectorName("Generate VRChat Toon Standard")]
        GenerateVRChatToonStandard = 0x01,
        [InspectorName("Generate VRChat Toon Standard (Outline)")]
        GenerateVRChatToonStandardOutline = 0x02,
        [InspectorName("Generate MToon")]
        GenerateMToon = 0x10,
        [InspectorName("Generate MToon10")]
        GenerateMToon10 = 0x11,
    }
}
