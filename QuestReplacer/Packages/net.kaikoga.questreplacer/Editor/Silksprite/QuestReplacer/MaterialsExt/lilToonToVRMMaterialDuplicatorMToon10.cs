using System.Diagnostics.CodeAnalysis;
using Silksprite.QuestReplacer.Materials;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class lilToonToVRMMaterialDuplicatorMToon10 : ISingleMaterialDuplicator
    {
        static readonly ISingleMaterialDuplicator _duplicatorMToon = new lilToonToVRMMaterialDuplicator();
        static readonly ISingleMaterialDuplicator _mToonUpgrader = new MToonUpgrader();

        bool ISingleMaterialDuplicator.IsTarget(Material original)
        {
            return _duplicatorMToon.IsTarget(original);
        }

        Material ISingleMaterialDuplicator.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var mtoon = _duplicatorMToon.Duplicate(original, bakedAssetDirectoryPath);
            var material = _mToonUpgrader.Duplicate(mtoon, bakedAssetDirectoryPath);
            return material;
        }
    }
}
