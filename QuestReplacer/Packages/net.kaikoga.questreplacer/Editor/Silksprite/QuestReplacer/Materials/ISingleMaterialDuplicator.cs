using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public interface ISingleMaterialDuplicator
    {
        int Priority { get; }
        bool IsTarget(Material original);
        Material Duplicate(Material original, string bakedAssetDirectoryPath);
    }
}
