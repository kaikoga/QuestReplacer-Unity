using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.Assets
{
    public abstract class AssetDuplicatorRepositoryBase<TAsset, TGenerationMode>
        where TAsset : Object
        where TGenerationMode : Enum
    {
        readonly Dictionary<TGenerationMode, SortedSet<ISingleAssetDuplicator<TAsset>>> _duplicators = CreateDictionary();

        static Dictionary<TGenerationMode, SortedSet<ISingleAssetDuplicator<TAsset>>> CreateDictionary() =>
            typeof(TGenerationMode).GetEnumValues().OfType<TGenerationMode>()
                .ToDictionary(
                    t => t,
                    _ => new SortedSet<ISingleAssetDuplicator<TAsset>>(SingleAssetDuplicatorComparer<TAsset>.Instance));

        public bool Register(TGenerationMode generationMode, ISingleAssetDuplicator<TAsset> duplicator)
        {
            if (!_duplicators.TryGetValue(generationMode, out var set))
            {
                return false;
            }
            set.Add(duplicator);
            return true;
        }

        protected IEnumerable<ISingleAssetDuplicator<TAsset>> Query(TGenerationMode generationMode) => _duplicators[generationMode];
    }

    class SingleAssetDuplicatorComparer<T> : IComparer<ISingleAssetDuplicator<T>>
        where T : Object
    {
        public static readonly SingleAssetDuplicatorComparer<T> Instance = new();
        public int Compare(ISingleAssetDuplicator<T> x, ISingleAssetDuplicator<T> y)
        {
            return Comparer.Default.Compare(y?.Priority, x?.Priority);
        }
    }
}
