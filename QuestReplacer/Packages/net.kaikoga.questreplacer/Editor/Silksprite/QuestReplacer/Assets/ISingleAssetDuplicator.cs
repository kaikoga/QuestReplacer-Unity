using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public interface ISingleAssetDuplicator<T>
    where T : Object
    {
        int Priority { get; }
        bool IsTarget(T original);
        bool TryDuplicate(T original, string bakedAssetDirectoryPath, out T result);
    }
}
