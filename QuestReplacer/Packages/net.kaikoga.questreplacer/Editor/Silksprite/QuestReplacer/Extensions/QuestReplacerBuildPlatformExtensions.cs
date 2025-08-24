using System;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacerBuildPlatformExtensions
    {
        public static bool Match(this QuestReplacerBuildPlatform buildPlatform, QuestReplacerPlatform platform) =>
            platform switch
            {
                QuestReplacerPlatform.Any => true,
                QuestReplacerPlatform.Generic => buildPlatform is QuestReplacerBuildPlatform.Generic,
                QuestReplacerPlatform.VRChatPC => buildPlatform is QuestReplacerBuildPlatform.VRChatPC,
                QuestReplacerPlatform.VRChatMobile => buildPlatform is QuestReplacerBuildPlatform.VRChatAndroid or QuestReplacerBuildPlatform.VRChatIos,
                QuestReplacerPlatform.VRChatAndroid => buildPlatform is QuestReplacerBuildPlatform.VRChatAndroid,
                QuestReplacerPlatform.VRChatIos => buildPlatform is QuestReplacerBuildPlatform.VRChatIos,
                QuestReplacerPlatform.VRM0 => buildPlatform is QuestReplacerBuildPlatform.VRM0,
                QuestReplacerPlatform.VRM1 => buildPlatform is QuestReplacerBuildPlatform.VRM1,
                _ => throw new ArgumentOutOfRangeException(nameof(buildPlatform), buildPlatform, null)
            };
    }
}