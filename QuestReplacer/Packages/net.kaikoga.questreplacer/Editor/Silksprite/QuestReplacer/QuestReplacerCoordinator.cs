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

        public void Execute(QuestReplacerPlatform platform)
        {
            foreach (var replacer in _replacers.Where(replacer => replacer.database && replacer.database.platform != platform))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(false);
            }
            foreach (var replacer in _replacers.Where(replacer => !replacer.database || replacer.database.platform == platform))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(true);
            }
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