using UnityEngine;

namespace Silksprite.QuestReplacer.Context.Commands
{
    public class SyncCommand : CommandBase
    {
        protected override string Name => "QuestReplacer: Sync";

        public SyncCommand(QuestReplacer questReplacer) : base(questReplacer)
        {
        }

        protected override void DoExecute()
        {
            var config = QuestReplacer.Config;
            QuestReplacer.pairs.Clear();
            new LoadFromDatabaseCommand(QuestReplacer).ExecuteWithoutUndo();
            if (config.manageMaterials)
            {
                new CollectCommand<Material>(QuestReplacer).ExecuteWithoutUndo();
                new GenerateMaterialsCommand(QuestReplacer).ExecuteWithoutUndo();
                new SaveToDatabaseCommand(QuestReplacer).ExecuteWithoutUndo();
            }
            if (config.manageMeshes)
            {
                new CollectCommand<Mesh>(QuestReplacer).ExecuteWithoutUndo();
            }
            if (config.manageAnimationClips)
            {
                new CollectCommand<AnimationClip>(QuestReplacer).ExecuteWithoutUndo();
            }
        }
    }
}