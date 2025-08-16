using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public interface ISingleMaterialDuplicator
    {
        bool IsTarget(Material original);
        Material Duplicate(Material original, string preferredPath);
    }
}
