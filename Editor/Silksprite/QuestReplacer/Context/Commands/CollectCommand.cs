using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class CollectCommand<T> : CommandBase
    where T : Object
    {
        protected override string Name => "QuestReplacer: Collect";

        public CollectCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            var references = Context.DeepCollectReferences<T>();
#if QUESTREPLACER_VRCSDK3_AVATARS
            if (references is IEnumerable<AnimationClip> clips)
            {
                references = clips.Where(clip => !clip.name.StartsWith("proxy_")).OfType<T>();
            }
#endif
            QuestReplacer.AddEntries(references, null, true);
        }
    }
}