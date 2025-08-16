using System.IO;
using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.MaterialsExt.Support;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt
{
    public class VRCQuestToolsMaterialDuplicator : ISingleMaterialDuplicator
    {
        bool ISingleMaterialDuplicator.IsTarget(Material original) => true;

        Material ISingleMaterialDuplicator.Duplicate(Material original, string preferredPath)
        {
            var material = VRCQuestToolsSupport.ConvertSingleMaterial(original, Path.GetDirectoryName(preferredPath));
            if (material != original) AssetDatabase.CreateAsset(material, preferredPath);
            return material;
        }
    }
}
