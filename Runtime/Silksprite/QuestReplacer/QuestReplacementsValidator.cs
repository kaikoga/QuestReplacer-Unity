using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.Loch;
using static Silksprite.Loch.Tools.LochTool;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacementsValidator
    {
        readonly QuestReplacement[] _replacements;
        readonly QuestReplacement[] _effectiveReplacements;
        List<string> _messages;

        public QuestReplacementsValidator(IEnumerable<QuestReplacement> replacements)
        {
            _replacements = replacements.ToArray();
            _effectiveReplacements = _replacements.Where(r => r.left != r.right).ToArray();
        }

        void Reset()
        {
            _messages = null;
        }

        void Message(string message)
        {
            _messages = _messages ?? new List<string>();
            _messages.Add(message);
        }

        void MessageIfAny<T>(LocalizedContent loc, IEnumerable<T> entries, Func<T, string> messageSelector)
        {
            var array = entries.ToArray();
            if (array.Length == 0) return;

            Message(loc.Tr);
            foreach (var entry in array)
            {
                Message(messageSelector(entry));
            }
        }

        void MessageIfAny(LocalizedContent loc, IEnumerable<UnityEngine.Object> entries) => MessageIfAny(loc, entries, o => o ? $"- {o.name}" : "- None");

        public bool DoValidate(out IEnumerable<string> messages)
        {
            Reset();

            var loosePairs = _replacements.Where(v => v.left && !v.right).Select(v => v.left)
                .Concat(_replacements.Where(v => v.right && !v.left).Select(v => v.right))
                .ToArray();
            MessageIfAny(Loc("QuestReplacementsValidator::LoosePairs."), loosePairs.Distinct());

            var duplicatesLeft = _replacements.Select(r => r.left).Where(o => o != null).Duplicate().ToArray();
            MessageIfAny(Loc("QuestReplacementsValidator::DuplicateLeft."), duplicatesLeft);
            var duplicatesRight = _replacements.Select(r => r.right).Where(o => o != null).Duplicate().ToArray();
            MessageIfAny(Loc("QuestReplacementsValidator::DuplicateRight."), duplicatesRight);

            var duplicatesEither = _effectiveReplacements.Select(r => r.left)
                .Join(_effectiveReplacements.Select(r => r.right),
                    l => l,
                    r => r,
                    (a, b) => a)
                .Distinct()
                .ToArray();
            MessageIfAny(Loc("QuestReplacementsValidator::DuplicateEither."), duplicatesEither);

            messages = _messages;
            return messages == null;
        }
    }

    static class QuestReplacementsValidatorExtensions {
        public static IEnumerable<T> Duplicate<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.GroupBy(e => e)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }
    }
}