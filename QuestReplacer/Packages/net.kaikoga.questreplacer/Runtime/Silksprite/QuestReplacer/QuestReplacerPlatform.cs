using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public enum QuestReplacerPlatform
    {
        Generic = 0x00,
        [InspectorName("VRChat PC")]
        VRChatPC = 0x100,
        [InspectorName("VRChat Mobile")]
        VRChatMobile = 0x110,
        [InspectorName("VRChat Android")]
        VRChatAndroid = 0x111,
        [InspectorName("VRChat iOS")]
        VRChatIos = 0x112,
        VRM0 = 0x200,
        VRM1 = 0x210,
    }
}
