using System;

namespace Silksprite.QuestReplacer
{
    public static class QuestReplacerEditorEvents
    {
        public static event Action<QuestReplacer> QuestReplacerDirty;
        
        internal static void OnQuestReplacerDirty(QuestReplacer obj)
        {
            QuestReplacerDirty?.Invoke(obj);
        }
    }
}