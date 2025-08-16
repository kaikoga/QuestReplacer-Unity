using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public static class Shaders
    {
        public static readonly Shader Standard = Shader.Find("Standard");
        public static readonly Shader VrcMobileStandardLite = Shader.Find("VRChat/Mobile/Standard Lite");
        public static readonly Shader VrcMobileToonLit = Shader.Find("VRChat/Mobile/Toon Lit");
        public static readonly Shader VrcMobileToonStandard = Shader.Find("VRChat/Mobile/Toon Standard");
        public static readonly Shader VrmMToon = Shader.Find("VRM/MToon");
        public static readonly Shader VrmMToon10 = Shader.Find("VRM10/MToon10");
    }
}