using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerCoordinator
    {
        readonly QuestReplacer[] _replacers;

        QuestReplacerCoordinator(IEnumerable<QuestReplacer> replacers)
        {
            _replacers = replacers.ToArray();
        }

        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform)
        {
            return new QuestReplacerCoordinator(avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true));
        }

        public void Execute(QuestReplacerBuildPlatform platform)
        {
            foreach (var replacer in _replacers.Where(replacer => !platform.Match(replacer.database?.platform)))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(false);
            }
            foreach (var replacer in _replacers.Where(replacer => platform.Match(replacer.database?.platform)))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(true);
            }
        }

        public bool Query<T>(T fromValue, QuestReplacerBuildPlatform platform, out T toValue)
        where T : Object
        {
            var replaced = false;
            toValue = fromValue;
            foreach (var replacer in _replacers.Where(replacer => !platform.Match(replacer.database?.platform)))
            {
                if (!replacer.ToContext().Query(toValue, false, out toValue)) continue;
                replaced = true;
            }
            foreach (var replacer in _replacers.Where(replacer => platform.Match(replacer.database?.platform)))
            {
                if (!replacer.ToContext().Query(toValue, true, out toValue)) continue;
                replaced = true;
            }
            return replaced;
        }

        public void DestroyImmediate()
        {
            foreach (var replacer in _replacers)
            {
                Object.DestroyImmediate(replacer);
            }
        }
    }
}