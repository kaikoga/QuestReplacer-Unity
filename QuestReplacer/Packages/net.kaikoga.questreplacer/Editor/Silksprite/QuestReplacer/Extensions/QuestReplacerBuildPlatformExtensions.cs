using System;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerBuildPlatformExtensions
    {
        public static bool Match(this QuestReplacerBuildPlatform buildPlatform, QuestReplacerPlatform? platform) =>
            buildPlatform switch
            {
                QuestReplacerBuildPlatform.Generic => platform is QuestReplacerPlatform.Generic,
                QuestReplacerBuildPlatform.VRChatPC => platform is QuestReplacerPlatform.VRChatPC,
                QuestReplacerBuildPlatform.VRChatAndroid => platform is QuestReplacerPlatform.VRChatAndroid or QuestReplacerPlatform.VRChatMobile,
                QuestReplacerBuildPlatform.VRChatIos => platform is QuestReplacerPlatform.VRChatIos or QuestReplacerPlatform.VRChatMobile,
                QuestReplacerBuildPlatform.VRM0 => platform is QuestReplacerPlatform.VRM0,
                QuestReplacerBuildPlatform.VRM1 => platform is QuestReplacerPlatform.VRM1,
                _ => throw new ArgumentOutOfRangeException(nameof(buildPlatform), buildPlatform, null)
            };
    }
}