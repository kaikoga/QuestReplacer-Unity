using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public enum QuestReplacerPlatform
    {
        [InspectorName("VRChat Mobile")]
        VRChatMobile = 0x00,
        [InspectorName("VRChat Android")]
        VRChatAndroid = 0x01,
        [InspectorName("VRChat iOS")]
        VRChatIos = 0x02,
        VRM0 = 0x10,
        VRM1 = 0x11,
    }
}
