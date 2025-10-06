using System;
using Silksprite.QuestReplacer.Context;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.Ndmf
{
    public class ReusableQuestReplacerCoordinator
    {
        internal QuestReplacerCoordinator Coordinator { get; }
        int _refCount;

        ReusableQuestReplacerCoordinator(QuestReplacerCoordinator coordinator)
        {
            Coordinator = coordinator;
        }

        public static ReusableQuestReplacerCoordinator FromAvatarRoot(Transform avatarRootTransform, bool cloneAnimations)
        {
            var coordinator = QuestReplacerCoordinatorFactory.FromAvatarRoot(avatarRootTransform, cloneAnimations);
            return new ReusableQuestReplacerCoordinator(coordinator);
        }

        public ReusableQuestReplacerCoordinatorReference Acquire()
        {
            _refCount++;
            return new ReusableQuestReplacerCoordinatorReference(this);
        }

        internal void Release()
        {
            if (--_refCount == 0) Coordinator.Dispose();
        }
    }

    public class ReusableQuestReplacerCoordinatorReference : IDisposable
    {
        readonly ReusableQuestReplacerCoordinator _reusable;

        internal ReusableQuestReplacerCoordinatorReference(ReusableQuestReplacerCoordinator reusable)
        {
            _reusable = reusable;
        }

        public bool Query<T>(T material, QuestReplacerBuildPlatform platform, out T o) where T : Object => _reusable.Coordinator.Query(material, platform, out o);

        public void Dispose()
        {
            _reusable.Release();
        }
    }
}
