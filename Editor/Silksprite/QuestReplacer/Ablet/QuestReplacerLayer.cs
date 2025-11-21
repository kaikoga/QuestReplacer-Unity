using Ablet.API;
using Ablet.API.V1;
using Ablet.API.V1.Attributes;
using Ablet.API.V1.Building;
using Ablet.Builtin;
using Silksprite.QuestReplacer.Platform;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ablet
{
    [AbletLayer]
    class QuestReplacerLayer : IAbletLayer
    {
        public string Id => "net.kaikoga.questreplacer";
        public string DisplayName => "QuestReplacer";

        public void Configure(IDependencyConfigurator config)
        {
            config.AddDependency<TransformingPhase>();
        }
        public AbletProcedure ToProcedure(IBuildArgument argument)
        {
            return AbletSymbols.PreferAblet ? new QuestReplacerProcedure() : null;
        }
    }

    class QuestReplacerProcedure : AbletObservableProcedure
    {
        public override void Observe(IObserveContext context) => DoObserve(context);

        void DoObserve(IObserveContext context)
        {
            context.RootTransform.Observe(transform =>
            {
                if (QuestReplacerPlatformDetector.TryGetPlatformForAvatar(transform, out var platform))
                {
                    DoExecute(transform, platform);
                }
            });
        }

        void DoExecute(Transform transform, QuestReplacerBuildPlatform platform)
        {
            using var coordinator = QuestReplacerCoordinatorFactory.FromAvatarRoot(transform, true);
            coordinator.Execute(platform, true);
            coordinator.DestroyImmediate();
        }
    }
}
