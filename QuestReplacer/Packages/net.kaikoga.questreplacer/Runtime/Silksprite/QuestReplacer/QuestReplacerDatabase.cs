using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    [CreateAssetMenu(fileName = "Quest Replacer Database", menuName = "Silksprite/Quest Replacer Database", order = 0)]
    public class QuestReplacerDatabase : ScriptableObject
    {
        public List<QuestTypeFilter> componentFilters = new List<QuestTypeFilter>();
        public List<QuestReplacement> pairs = new List<QuestReplacement>();

        public const bool ManageMaterialsDefault = true;
        public const bool ManageMeshesDefault = false;
        
        public bool manageMaterials = ManageMaterialsDefault;
        public bool manageMeshes = ManageMeshesDefault;

        public Platform platform = Platform.VRChatMobile;
        public GenerateMode generateMode = GenerateMode.VRChatToonStandard;
        public string generatedDirectory = "";
        public string generatedFilePrefix = "";
        public string generatedFileSuffix = "-q";

        static bool _instanceFound;
        static QuestReplacerDatabase _instance;

        public enum Platform
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

        public enum GenerateMode
        {
            [InspectorName("VRChat ToonLit")]
            VRChatToonLit = 0x00,
            [InspectorName("VRChat Toon Standard")]
            VRChatToonStandard = 0x01,
            [InspectorName("MToon")]
            MToon = 0x10,
            [InspectorName("MToon10")]
            MToon10 = 0x11,
        }

        public void RegisterTypeFilters(IEnumerable<Type> types)
        {
            componentFilters = componentFilters.Merge(types).ToList();
        }
    }
}