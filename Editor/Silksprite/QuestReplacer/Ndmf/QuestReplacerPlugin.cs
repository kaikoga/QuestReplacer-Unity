using System;
using Ablet.API;
using nadena.dev.ndmf;
using Silksprite.QuestReplacer.Ndmf;
using Silksprite.QuestReplacer.Platform;
using UnityEngine;

[assembly: ExportsPlugin(typeof(QuestReplacerPlugin))]

namespace Silksprite.QuestReplacer.Ndmf
{
    // runs independently of NDMF platform
    [RunsOnAllPlatforms]
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
            transforming.Run(QuestReplacerPass.Instance)
                .PreviewingWith(new QuestReplacerPreview());
        }
    }

    class QuestReplacerPass : Pass<QuestReplacerPass>
    {
        protected override void Execute(BuildContext buildContext)
        {
#if QUESTREPLACER_ABLET_SUPPORT
            if (AbletSymbols.PreferAblet) return;
#endif
            if (QuestReplacerPlatformDetector.TryGetPlatformForAvatar(buildContext.AvatarRootTransform, out var platform))
            {
                DoExecute(buildContext, platform);
            }
        }

        void DoExecute(BuildContext buildContext, QuestReplacerBuildPlatform platform)
        {
            using var coordinator = QuestReplacerCoordinatorFactory.FromAvatarRoot(buildContext.AvatarRootTransform, true);
            coordinator.Execute(platform, true);
            coordinator.DestroyImmediate();
        }
    }
}
