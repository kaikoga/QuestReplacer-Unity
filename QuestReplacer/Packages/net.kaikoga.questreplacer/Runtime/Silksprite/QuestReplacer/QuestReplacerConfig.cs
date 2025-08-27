using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    [Serializable]
    public class QuestReplacerConfig
    {
        public QuestReplacerPlatform platform = QuestReplacerPlatform.VRChatMobile;
        public QuestReplacerGenerateMode generateMode = QuestReplacerGenerateMode.GenerateVRChatToonStandard;

        public bool manageMaterials = true;
        public bool manageMeshes;
    }
}