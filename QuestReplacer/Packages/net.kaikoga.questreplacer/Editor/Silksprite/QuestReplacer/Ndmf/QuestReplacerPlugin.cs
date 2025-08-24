#if QUEST_REPLACER_NDMF_SUPPORT

using System;
using System.Linq;
using nadena.dev.ndmf;
using Silksprite.QuestReplacer.Extensions;
using Silksprite.QuestReplacer.Ndmf;
using UnityEngine;
using Object = UnityEngine.Object;

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
            if (QuestReplacerPlatformDetector.TryGetPlatformForAvatar(buildContext.AvatarRootObject, out var platform))
            {
                DoExecute(buildContext, platform);
            }
        }
        
        void DoExecute(BuildContext buildContext, QuestReplacerPlatform platform)
        {
            var coordinator = QuestReplacerCoordinator.FromAvatarRoot(buildContext.AvatarRootTransform);
            coordinator.Execute(platform);
            coordinator.DestroyImmediate();
        }
    }
}

#endif