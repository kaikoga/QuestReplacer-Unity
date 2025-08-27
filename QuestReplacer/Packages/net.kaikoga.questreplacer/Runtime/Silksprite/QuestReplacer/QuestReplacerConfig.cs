using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Silksprite.QuestReplacer
{
    [Serializable]
    public class QuestReplacerConfig
    {
        public QuestReplacerPlatform platform = QuestReplacerPlatform.VRChatMobile;
        public QuestReplacerMaterialGenerationMode materialGenerationMode = QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard;

        public bool manageMaterials = true;
        public bool manageMeshes;
        
        public bool targetVRChatAnimations = true;
    }
}