using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class SingleAssetDuplicator<T> : ISingleAssetDuplicator<T>
    where T : Object
    {
        int ISingleAssetDuplicator<T>.Priority => 0;

        bool ISingleAssetDuplicator<T>.IsTarget(T original) => true;

        bool ISingleAssetDuplicator<T>.TryDuplicate(T original, string bakedAssetDirectoryPath, out T result)
        {
            result = Object.Instantiate(original);
            return true;
        }
    }
}