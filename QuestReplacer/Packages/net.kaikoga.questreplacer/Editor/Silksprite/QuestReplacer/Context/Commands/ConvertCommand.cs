using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ConvertCommand : CommandBase
    {
        protected override string Name => _toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left";
        protected override bool InvalidateTypeFilters => false;

        readonly bool _toRight;

        public ConvertCommand(QuestReplacer questReplacer, bool toRight) : base(questReplacer)
        {
            _toRight = toRight;
        }
        protected override void DoExecute()
        {
            QuestReplacerContext context = Context;
            UpdateTypeFilters(QuestReplacer, context);
            context.DeepOverrideReferences<Object>(_toRight, withAssets: false);
        }
    }
}