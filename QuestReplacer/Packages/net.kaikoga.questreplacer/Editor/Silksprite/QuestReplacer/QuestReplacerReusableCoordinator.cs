using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerReusableCoordinator
    {
        readonly Transform _avatarRootTransform;

        QuestReplacerCoordinator _coordinator;
        public QuestReplacerCoordinator Coordinator => _coordinator ??= QuestReplacerCoordinator.FromAvatarRoot(_avatarRootTransform);

        public void SetDirty() => _coordinator = null;

        QuestReplacerReusableCoordinator(Transform avatarRootTransform)
        {
            _avatarRootTransform = avatarRootTransform;
        }
        
        public static QuestReplacerReusableCoordinator FromAvatarRoot(Transform avatarRootTransform)
        {
            return new QuestReplacerReusableCoordinator(avatarRootTransform);
        }
    }
}