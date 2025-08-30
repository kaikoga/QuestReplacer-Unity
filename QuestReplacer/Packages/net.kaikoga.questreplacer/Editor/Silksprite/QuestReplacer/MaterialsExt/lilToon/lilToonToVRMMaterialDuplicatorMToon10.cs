using System.Diagnostics.CodeAnalysis;
using Silksprite.QuestReplacer.Assets;
using Silksprite.QuestReplacer.Materials.VRMShaders;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class lilToonToVRMMaterialDuplicatorMToon10 : ISingleAssetDuplicator<Material>
    {
        static readonly ISingleAssetDuplicator<Material> _duplicatorMToon = new lilToonToVRMMaterialDuplicator();
        static readonly ISingleAssetDuplicator<Material> _mToonUpgrader = new MToonUpgrader();

        int ISingleAssetDuplicator<Material>.Priority => 10000;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original)
        {
            return _duplicatorMToon.IsTarget(original);
        }

        Material ISingleAssetDuplicator<Material>.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var mtoon = _duplicatorMToon.Duplicate(original, bakedAssetDirectoryPath);
            var material = _mToonUpgrader.Duplicate(mtoon, bakedAssetDirectoryPath);
            return material;
        }
    }
}
