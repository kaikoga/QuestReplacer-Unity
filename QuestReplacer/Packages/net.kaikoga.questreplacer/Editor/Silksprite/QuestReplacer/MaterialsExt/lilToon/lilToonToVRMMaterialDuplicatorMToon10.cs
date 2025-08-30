using System.Diagnostics.CodeAnalysis;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

#if QUESTREPLACER_VRMSHADERS || QUESTREPLACER_UNIVRM_WITH_SHADERS
using Silksprite.QuestReplacer.MaterialsExt.VRMShaders;
#endif

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class lilToonToVRMMaterialDuplicatorMToon10 : ISingleAssetDuplicator<Material>
    {
        static readonly ISingleAssetDuplicator<Material> _duplicatorMToon = new lilToonToVRMMaterialDuplicator();
#if QUESTREPLACER_VRMSHADERS || QUESTREPLACER_UNIVRM_WITH_SHADERS
        static readonly ISingleAssetDuplicator<Material> _mToonUpgrader = new MToonUpgrader();
#endif
        int ISingleAssetDuplicator<Material>.Priority => 10000;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original)
        {
            return _duplicatorMToon.IsTarget(original);
        }

        Material ISingleAssetDuplicator<Material>.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var mtoon = _duplicatorMToon.Duplicate(original, bakedAssetDirectoryPath);
#if QUESTREPLACER_VRMSHADERS || QUESTREPLACER_UNIVRM_WITH_SHADERS
            var material = _mToonUpgrader.Duplicate(mtoon, bakedAssetDirectoryPath);
            return material;
#else
            return mtoon;
#endif
        }
    }
}
