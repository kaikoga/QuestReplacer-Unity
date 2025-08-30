using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class lilToonToVRMMaterialDuplicatorMToon10 : ISingleAssetDuplicator<Material>
    {
        static readonly ISingleAssetDuplicator<Material>[] _duplicators =
        {
            new lilToonToVRMMaterialDuplicator(),
#if QUESTREPLACER_VRMSHADERS || QUESTREPLACER_UNIVRM_WITH_SHADERS
            new VRMShaders.MToonUpgrader(),
#endif
        };

        int ISingleAssetDuplicator<Material>.Priority => 10000;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original)
        {
            var originalShaderName = original.shader.name;
            return originalShaderName.Contains("lilToon") || originalShaderName.StartsWith("Hidden/lts");
        }

        Material ISingleAssetDuplicator<Material>.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            return _duplicators.Aggregate(original, (material, duplicator) => duplicator.Duplicate(material, bakedAssetDirectoryPath));
        }
    }
}
