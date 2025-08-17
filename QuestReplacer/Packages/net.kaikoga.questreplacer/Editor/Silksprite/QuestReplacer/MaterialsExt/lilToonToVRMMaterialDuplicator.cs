using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.MaterialsExt.Support;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt
{
    public class lilToonToVRMMaterialDuplicator : ISingleMaterialDuplicator
    {
        readonly MToonUpgrader _mToonUpgrader;

        public lilToonToVRMMaterialDuplicator(bool isVrm1)
        {
            _mToonUpgrader = isVrm1 ? new MToonUpgrader() : null; 
        }

        bool ISingleMaterialDuplicator.IsTarget(Material original)
        {
            var originalShaderName = original.shader.name;
            return originalShaderName.Contains("lilToon") || originalShaderName.StartsWith("Hidden/lts");
        }

        Material ISingleMaterialDuplicator.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var material = lilToonSupport.lilDuplicateMaterial(original, bakedAssetDirectoryPath);
            if (_mToonUpgrader != null)
            {
                material = _mToonUpgrader.Duplicate(material, bakedAssetDirectoryPath);
            }
            return material;
        }
    }
}
