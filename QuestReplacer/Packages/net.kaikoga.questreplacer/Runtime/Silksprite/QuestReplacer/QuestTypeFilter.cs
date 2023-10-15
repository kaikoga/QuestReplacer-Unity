using System;
using System.Collections.Generic;

namespace Silksprite.QuestReplacer
{
    [Serializable]
    public struct QuestTypeFilter
    {
        public string typePrefix;
        public bool isIncluded;

        public bool Match(Type type)
        {
            var typeFullName = type.FullName;
            return !string.IsNullOrEmpty(typeFullName) && typeFullName.StartsWith(typePrefix);
        }

        public bool Includes(Type type) => isIncluded && Match(type);
    }
}