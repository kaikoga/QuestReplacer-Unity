using Silksprite.QuestReplacer.Materials;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public class VRCQuestToolsMaterialDuplicator : ISingleMaterialDuplicator
    {
        bool ISingleMaterialDuplicator.IsTarget(Material original) => true;

        Material ISingleMaterialDuplicator.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var material = VRCQuestToolsSupport.ConvertSingleMaterial(original, bakedAssetDirectoryPath);
            return material;
        }
    }
}
