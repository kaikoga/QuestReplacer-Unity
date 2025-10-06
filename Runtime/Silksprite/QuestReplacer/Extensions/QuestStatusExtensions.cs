using System;

namespace Silksprite.QuestReplacer.Extensions
{
    public static class QuestStatusExtensions
    {
        public static QuestStatus Merge(this QuestStatus lhs, QuestStatus rhs)
        {
            switch (lhs)
            {
                case QuestStatus.Unmanaged:
                    return QuestStatus.Unmanaged;
                case QuestStatus.Left:
                    switch (rhs)
                    {
                        case QuestStatus.Unmanaged:
                            return QuestStatus.Unmanaged;
                        case QuestStatus.Left:
                            return QuestStatus.Left;
                        case QuestStatus.Right:
                            return QuestStatus.Mixed;
                        case QuestStatus.Either:
                            return QuestStatus.Left;
                        case QuestStatus.Mixed:
                            return QuestStatus.Mixed;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rhs), rhs, null);
                    }
                case QuestStatus.Right:
                    switch (rhs)
                    {
                        case QuestStatus.Unmanaged:
                            return QuestStatus.Unmanaged;
                        case QuestStatus.Left:
                            return QuestStatus.Mixed;
                        case QuestStatus.Right:
                            return QuestStatus.Right;
                        case QuestStatus.Either:
                            return QuestStatus.Right;
                        case QuestStatus.Mixed:
                            return QuestStatus.Mixed;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rhs), rhs, null);
                    }
                case QuestStatus.Either:
                    switch (rhs)
                    {
                        case QuestStatus.Unmanaged:
                            return QuestStatus.Unmanaged;
                        case QuestStatus.Left:
                            return QuestStatus.Left;
                        case QuestStatus.Right:
                            return QuestStatus.Right;
                        case QuestStatus.Either:
                            return QuestStatus.Either;
                        case QuestStatus.Mixed:
                            return QuestStatus.Mixed;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rhs), rhs, null);
                    }
                case QuestStatus.Mixed:
                    switch (rhs)
                    {
                        case QuestStatus.Unmanaged:
                            return QuestStatus.Unmanaged;
                        case QuestStatus.Left:
                        case QuestStatus.Right:
                        case QuestStatus.Either:
                        case QuestStatus.Mixed:
                            return QuestStatus.Mixed;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rhs), rhs, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(lhs), lhs, null);
            }
        }
    }
}