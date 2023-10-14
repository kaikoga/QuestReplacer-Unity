using System.Collections.Generic;
using System.Linq;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestReplacementsExtensions
    {
        public static bool Validate(this IEnumerable<QuestReplacement> self, out IEnumerable<string> messages) => new QuestReplacementsValidator(self).DoValidate(out messages);

        public static IEnumerable<QuestReplacement> Merge(this IEnumerable<QuestReplacement> self, IEnumerable<QuestReplacement> other)
        {
            var others = other.ToArray();
            foreach (var r in self)
            {
                if (others.All(o => !r.Contains(o))) yield return r;
            }

            foreach (var r in others) yield return r;
        }

        public static IEnumerable<QuestReplacement> Update(this IEnumerable<QuestReplacement> self, IEnumerable<QuestReplacement> other)
        {
            var others = other.ToArray();
            foreach (var r in self)
            {
                yield return others.FirstOrDefault(o => r.Contains(o)) ?? r;
            }
        }
    }
}