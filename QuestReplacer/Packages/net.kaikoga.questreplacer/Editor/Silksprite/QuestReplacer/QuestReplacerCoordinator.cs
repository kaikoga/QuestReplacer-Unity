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
        readonly (QuestReplacerPlatform platform, QuestReplacerContext context)[] _contexts;

        QuestReplacerCoordinator(Transform avatarRootTransform, IEnumerable<QuestReplacer> replacers)
        {
            _replacers = replacers.ToArray();
            _contexts = _replacers
                .Select(replacer => (replacer.Platform, replacer.ToAvatarContext(avatarRootTransform)))
                .ToArray();
        }

        public static QuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform)
        {
            return new QuestReplacerCoordinator(avatarRootTransform,
                avatarRootTransform.GetComponentsInChildren<QuestReplacer>(true));
        }

        public void Execute(QuestReplacerBuildPlatform platform)
        {
            foreach (var context in _contexts.Where(context => !platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(false);
            }
            foreach (var context in _contexts.Where(context => platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(true);
            }
        }

        public bool Query<T>(T fromValue, QuestReplacerBuildPlatform platform, out T toValue)
        where T : Object
        {
            var replaced = false;
            toValue = fromValue;
            foreach (var context in _contexts.Where(context => !platform.Match(context.platform)))
            {
                if (!context.context.Query(toValue, false, out toValue)) continue;
                replaced = true;
            }
            foreach (var context in _contexts.Where(context => platform.Match(context.platform)))
            {
                if (!context.context.Query(toValue, true, out toValue)) continue;
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