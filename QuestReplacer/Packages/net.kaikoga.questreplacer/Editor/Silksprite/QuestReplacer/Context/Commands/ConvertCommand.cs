using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public static class ConvertCommand
    {
        public static void DoConvert(bool toRight, QuestReplacer questReplacer, QuestReplacerContext context)
        {
            Undo.SetCurrentGroupName(toRight ? "QuestReplacer: To Right" : "QuestReplacer: To Left");
            CommandBase.UpdateTypeFilters(questReplacer, context);
            context.DeepOverrideReferences<Object>(toRight, withAssets: false);
        }
    }
}