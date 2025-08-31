using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.Context
{
    public class QuestReplacerCoordinator : IDisposable
    {
        readonly QuestReplacer[] _replacers;
        readonly (QuestReplacerPlatform platform, bool animations, QuestReplacerContext context)[] _contexts;

        public QuestReplacerCoordinator(IEnumerable<QuestReplacer> replacers, bool cloneAnimations)
        {
            _replacers = replacers.ToArray();
            _contexts = _replacers
                .Select(replacer => (replacer.Platform, replacer.Config.targetVRChatAnimations, replacer.ToContext(cloneAnimations)))
                .ToArray();
        }

        public void Execute(QuestReplacerBuildPlatform platform, bool withAssets)
        {
            foreach (var context in _contexts.Where(context => !platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(toRight: false, withAssets: withAssets && context.animations);
            }
            foreach (var context in _contexts.Where(context => platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(toRight: true, withAssets: withAssets && context.animations);
            }
        }

        public bool Query<T>(T fromValue, QuestReplacerBuildPlatform platform, out T toValue)
        where T : Object
        {
            var replaced = false;
            toValue = fromValue;
            foreach (var context in _contexts.Where(context => !platform.Match(context.platform)))
            {
                if (!context.context.Query(toValue, false, out var value)) continue;
                toValue = value;
                replaced = true;
            }
            foreach (var context in _contexts.Where(context => platform.Match(context.platform)))
            {
                if (!context.context.Query(toValue, true, out var value)) continue;
                toValue = value;
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

        public void Dispose()
        {
            foreach (var (_, _, context) in _contexts)
            {
                context.Dispose();
            }
        }
    }
}