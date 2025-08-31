using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class ConvertCommand : CommandBase
    {
        readonly bool _toRight;

        public ConvertCommand(QuestReplacer questReplacer, QuestReplacerContext context, bool toRight) : base(questReplacer, context)
        {
            _toRight = toRight;
        }
        protected override void DoExecute(QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName(_toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            UpdateTypeFilters(questReplacer, context);
            context.DeepOverrideReferences<Object>(_toRight, withAssets: false);
        }
    }
}