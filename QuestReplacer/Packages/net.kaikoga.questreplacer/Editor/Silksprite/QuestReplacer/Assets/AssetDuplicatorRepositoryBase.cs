using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public abstract class AssetDuplicatorRepositoryBase<T>
        where T : Object
    {
        public bool RegisterExt(QuestReplacerMaterialGenerationMode materialGenerationMode, ISingleAssetDuplicator<T> duplicator)
        {
            if (!Exts.TryGetValue(materialGenerationMode, out var list))
            {
                return false;
            }
            list.Add(duplicator);
            return true;
        }

        protected readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<T>>> Exts = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleAssetDuplicator<T>>>
        {
            [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new SortedSet<ISingleAssetDuplicator<T>>(SingleAssetDuplicatorComparer<T>.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon] = new SortedSet<ISingleAssetDuplicator<T>>(SingleAssetDuplicatorComparer<T>.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon10] = new SortedSet<ISingleAssetDuplicator<T>>(SingleAssetDuplicatorComparer<T>.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard] = new SortedSet<ISingleAssetDuplicator<T>>(SingleAssetDuplicatorComparer<T>.Instance),
        };
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
