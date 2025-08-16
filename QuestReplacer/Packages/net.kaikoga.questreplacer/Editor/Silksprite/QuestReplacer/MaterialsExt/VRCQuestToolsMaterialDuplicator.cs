using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.MaterialsExt.Support;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt
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
