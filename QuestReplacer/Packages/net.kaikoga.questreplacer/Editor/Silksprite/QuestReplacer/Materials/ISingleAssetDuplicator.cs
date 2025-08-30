using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public interface ISingleAssetDuplicator<T>
    where T : Object
    {
        int Priority { get; }
        bool IsTarget(T original);
        T Duplicate(T original, string bakedAssetDirectoryPath);
    }
}
