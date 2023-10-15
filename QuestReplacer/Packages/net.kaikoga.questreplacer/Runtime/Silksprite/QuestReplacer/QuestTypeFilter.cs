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
            return !string.IsNullOrEmpty(type.FullName) && type.FullName.StartsWith(typePrefix);
        }

        public bool Includes(Type type) => isIncluded && Match(type);
    }
}