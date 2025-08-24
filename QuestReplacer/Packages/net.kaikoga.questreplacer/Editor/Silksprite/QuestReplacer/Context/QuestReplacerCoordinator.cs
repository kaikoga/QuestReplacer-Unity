using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.Context
{
    public class QuestReplacerCoordinator
    {
        readonly QuestReplacer[] _replacers;
        readonly (QuestReplacerPlatform platform, QuestReplacerContext context)[] _contexts;

        public QuestReplacerCoordinator(Transform avatarRootTransform, IEnumerable<AnimatorController> animatorControllers, IEnumerable<QuestReplacer> replacers)
        {
            _replacers = replacers.ToArray();
            var animatorControllersArray = animatorControllers.ToArray();
            _contexts = _replacers
                .Select(replacer => (replacer.Platform, replacer.ToAvatarContext(avatarRootTransform, animatorControllersArray)))
                .ToArray();
        }

        public void Execute(QuestReplacerBuildPlatform platform)
        {
            foreach (var context in _contexts.Where(context => !platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(toRight: false, withAssets: false);
            }
            foreach (var context in _contexts.Where(context => platform.Match(context.platform)))
            {
                context.context.DeepOverrideReferences<Object>(toRight: true, withAssets: false);
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