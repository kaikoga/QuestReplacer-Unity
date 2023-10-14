using System;
using System.Collections.Generic;
using System.Linq;

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

        void MessageIfAny<T>(string header, IEnumerable<T> entries, Func<T, string> messageSelector)
        {
            var array = entries.ToArray();
            if (array.Length == 0) return;

            Message(header);
            foreach (var entry in array)
            {
                Message(messageSelector(entry));
            }
        }

        void MessageIfAny(string header, IEnumerable<UnityEngine.Object> entries) => MessageIfAny(header, entries, o => o ? $"- {o.name}" : "- None");

        public bool DoValidate(out IEnumerable<string> messages)
        {
            Reset();

            var loosePairs = _replacements.Where(v => v.left && !v.right).Select(v => v.left)
                .Concat(_replacements.Where(v => v.right && !v.left).Select(v => v.right))
                .ToArray();
            MessageIfAny("ペアの項目が未設定です。", loosePairs.Distinct());

            var duplicatesLeft = _replacements.Select(r => r.left).Where(o => o != null).Duplicate().ToArray();
            MessageIfAny("ペアの左側の項目が重複しています。", duplicatesLeft);
            var duplicatesRight = _replacements.Select(r => r.right).Where(o => o != null).Duplicate().ToArray();
            MessageIfAny("ペアの右側の項目が重複しています。", duplicatesRight);

            var duplicatesEither = _effectiveReplacements.Select(r => r.left)
                .Join(_effectiveReplacements.Select(r => r.right),
                    l => l,
                    r => r,
                    (a, b) => a)
                .Distinct()
                .ToArray();
            MessageIfAny("項目がペアの左右両方に存在します。", duplicatesEither);

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