using System;
using Silksprite.QuestReplacer.Assets;

namespace Silksprite.QuestReplacer
{
    [Serializable]
    public class QuestReplacerConfig
    {
        public QuestReplacerPlatform platform = QuestReplacerPlatform.VRChatMobile;
        public QuestReplacerMaterialGenerationMode materialGenerationMode = QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard;

        public bool manageMaterials = true;
        public bool manageMeshes;
        public bool manageAnimationClips;
        
        public bool targetVRChatAnimations = true;
    }
}