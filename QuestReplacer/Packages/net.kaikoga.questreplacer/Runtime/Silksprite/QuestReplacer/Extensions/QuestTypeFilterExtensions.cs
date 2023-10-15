using System;
using System.Collections.Generic;
using System.Linq;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestTypeFilterExtensions
    {
        public static IEnumerable<QuestTypeFilter> Merge(this List<QuestTypeFilter> self, IEnumerable<Type> types)
        {
            return self.Concat(types
                    .Where(type => self.All(typeFilter => !typeFilter.Match(type)))
                    .ToTypeFilters())
                .Where(typeFilter => !string.IsNullOrWhiteSpace(typeFilter.typePrefix))
                .OrderBy(typeFilter => typeFilter.typePrefix);
        }

        public static IEnumerable<QuestTypeFilter> ToTypeFilters(this IEnumerable<Type> self)
        {
            return self.Select(type => type.Namespace)
                .Distinct()
                .Select(ns => new QuestTypeFilter
                {
                    typePrefix = ns,
                    isIncluded = !string.IsNullOrEmpty(ns) && ns != typeof(QuestReplacer).Namespace
                });
        }
    }
}