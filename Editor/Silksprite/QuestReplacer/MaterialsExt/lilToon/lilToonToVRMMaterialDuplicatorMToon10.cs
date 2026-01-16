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

        bool ISingleAssetDuplicator<Material>.TryDuplicate(Material original, string bakedAssetDirectoryPath, out Material result)
        {
            result = _duplicators.Aggregate(original, (material, duplicator) =>
            {
                duplicator.TryDuplicate(material, bakedAssetDirectoryPath, out var r);
                return r;
            });
            return true;
        }
    }
}
