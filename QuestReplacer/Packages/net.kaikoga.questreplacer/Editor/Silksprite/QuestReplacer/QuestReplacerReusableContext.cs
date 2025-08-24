using System;
using System.Collections.Generic;
using Silksprite.QuestReplacer.Extensions;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer
{
    public class QuestReplacerReusableContext
    {
        readonly QuestReplacer _questReplacer;

        QuestReplacerContext _context;
        QuestReplacerContext Context => _context ??= _questReplacer.ToContext();

        public void SetDirty()
        {
            _context = null;
            QuestReplacerEditorEvents.OnQuestReplacerDirty(_questReplacer); 
        }

        public QuestStatus QuestMaterialStatus => Context.QuestMaterialStatus;
        public QuestStatus QuestMeshStatus => Context.QuestMeshStatus;

        public QuestReplacerReusableContext(QuestReplacer questReplacer)
        {
            _questReplacer = questReplacer;
        }

        public IEnumerable<Type> DeepCollectComponentTypes()
        {
            return Context.DeepCollectComponentTypes();
        }

        public IEnumerable<T> DeepCollectReferences<T>() where T : Object
        {
            return Context.DeepCollectReferences<T>();
        }

        public void DeepOverrideReferences<T>(bool toRight) where T : Object
        {
            Context.DeepOverrideReferences<T>(toRight);
        }
    }
}