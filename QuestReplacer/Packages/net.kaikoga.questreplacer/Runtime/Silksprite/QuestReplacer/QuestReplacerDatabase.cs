using System;
using System.Collections.Generic;
using System.Linq;
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

        public GenerateMode generateMode = GenerateMode.Quest;
        public string generatedDirectory = "";
        public string generatedFilePrefix = "";
        public string generatedFileSuffix = "-q";

        static bool _instanceFound;
        static QuestReplacerDatabase _instance;

        public enum GenerateMode
        {
            Quest,
            VRM0,
            VRM1
        }

        public void RegisterTypeFilters(IEnumerable<Type> types)
        {
            componentFilters = componentFilters.Concat(types
                    .Where(type => componentFilters.All(typeFilter => !typeFilter.Match(type)))
                    .Select(type => type.Namespace)
                    .Distinct()
                    .Select(QuestTypeFilter.FromNamespace))
                .Where(typeFilter => !string.IsNullOrWhiteSpace(typeFilter.typePrefix))
                .OrderBy(typeFilter => typeFilter.typePrefix)
                .ToList();
        }
    }
}