using System;

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
            return isIncluded && !string.IsNullOrEmpty(typeFullName) && typeFullName.StartsWith(typePrefix);
        }

        public static QuestTypeFilter FromNamespace(string ns) => new QuestTypeFilter
        {
            typePrefix = ns,
            isIncluded = !string.IsNullOrEmpty(ns) && ns != typeof(QuestReplacer).Namespace
        };
    }
}