using System;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    [Serializable]
    public class QuestReplacement
    {
        public Object left;
        public Object right;

        public bool LikelyUnset => left == right || (left && !right);
        public bool Contains(Object target) => left == target || right == target;
        public bool Contains(QuestReplacement other) => Contains(other.left) || Contains(other.right);

        public QuestStatus GetStatus(Object currentObject)
        {
            var isLeft = currentObject == left;
            var isRight = currentObject == right;
            switch (isLeft)
            {
                case true:
                    switch (isRight)
                    {
                        case true: return QuestStatus.Either;
                        default: return QuestStatus.Left;
                    }
                default:
                    switch (isRight)
                    {
                        case true: return QuestStatus.Right;
                        default: return QuestStatus.Unmanaged;
                    }
            }
        }
    }
}