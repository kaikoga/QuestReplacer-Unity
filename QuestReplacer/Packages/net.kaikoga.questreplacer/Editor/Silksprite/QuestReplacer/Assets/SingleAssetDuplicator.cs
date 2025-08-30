using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class SingleAssetDuplicator<T> : ISingleAssetDuplicator<T>
    where T : Object
    {
        int ISingleAssetDuplicator<T>.Priority => 0;

        bool ISingleAssetDuplicator<T>.IsTarget(T original) => true;

        T ISingleAssetDuplicator<T>.Duplicate(T original, string bakedAssetDirectoryPath)
        {
            return Object.Instantiate(original);
        }
    }
}