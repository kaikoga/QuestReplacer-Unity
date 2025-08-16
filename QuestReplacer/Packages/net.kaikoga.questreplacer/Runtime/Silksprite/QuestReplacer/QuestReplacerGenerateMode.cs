using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public enum QuestReplacerGenerateMode
    {
        [InspectorName("VRChat ToonLit")]
        VRChatToonLit = 0x00,
        [InspectorName("VRChat Toon Standard")]
        VRChatToonStandard = 0x01,
        [InspectorName("VRChat Toon Standard (Outline)")]
        VRChatToonStandardOutline = 0x02,
        [InspectorName("MToon")]
        MToon = 0x10,
        [InspectorName("MToon10")]
        MToon10 = 0x11,
    }
}
