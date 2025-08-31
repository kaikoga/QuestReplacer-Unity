using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ConvertCommand : CommandBase
    {
        protected override string Name => _toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left";
        protected override bool InvalidateTypeFilters => false;

        readonly bool _toRight;

        public ConvertCommand(QuestReplacer questReplacer, QuestReplacerContext context, bool toRight) : base(questReplacer, context)
        {
            _toRight = toRight;
        }
        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            UpdateTypeFilters(questReplacer, context);
            context.DeepOverrideReferences<Object>(_toRight, withAssets: false);
        }
    }
}