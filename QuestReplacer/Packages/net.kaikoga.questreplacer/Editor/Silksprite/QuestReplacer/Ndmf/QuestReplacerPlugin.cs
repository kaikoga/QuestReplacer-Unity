#if QUEST_REPLACER_NDMF_SUPPORT

using System;
using nadena.dev.ndmf;
using UnityEngine;
using Silksprite.QuestReplacer.Ndmf;

[assembly: ExportsPlugin(typeof(QuestReplacerPlugin))]

namespace Silksprite.QuestReplacer.Ndmf
{
    class QuestReplacerPlugin : Plugin<QuestReplacerPlugin>
    {
        public override string QualifiedName => "net.kaikoga.questreplacer";
        public override string DisplayName => "QuestReplacer";

        protected override void OnUnhandledException(Exception e)
        {
            Debug.LogException(e);
        }

        protected override void Configure()
        {
            var transforming = InPhase(BuildPhase.Transforming);
            transforming.Run(QuestReplacerPass.Instance);
        }
    }

    class QuestReplacerPass : Pass<QuestReplacerPass>
    {
        protected override void Execute(BuildContext buildContext)
        {
            foreach (var root in buildContext.AvatarRootTransform.GetComponentsInChildren<QuestReplacer>(true))
            {
                // TODO
            }
        }
    }

}

#endif