using nadena.dev.ndmf.preview;
using UnityEngine;

namespace Silksprite.QuestReplacer.Ndmf
{
    static class QuestReplacerInvalidator
    {
        [RuntimeInitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            QuestReplacerEditorEvents.QuestReplacerDirty += OnQuestReplacerDirty;
        }

        static void OnQuestReplacerDirty(QuestReplacer questReplacer)
        {
            ChangeNotifier.NotifyObjectUpdate(questReplacer);
        }
    }
}
