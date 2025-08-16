using System.IO;
using Silksprite.QuestReplacer.Materials;
using Silksprite.QuestReplacer.MaterialsExt.Support;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt
{
    public class lilToonToVRMMaterialDuplicator : ISingleMaterialDuplicator
    {
        readonly Shader _shader;

        public lilToonToVRMMaterialDuplicator(Shader shader = null)
        {
            _shader = shader;
        }

        bool ISingleMaterialDuplicator.IsTarget(Material original)
        {
            var originalShaderName = original.shader.name;
            return originalShaderName.Contains("lilToon") || originalShaderName.StartsWith("Hidden/lts");
        }

        Material ISingleMaterialDuplicator.Duplicate(Material original, string preferredPath)
        {
            var material = lilToonSupport.lilDuplicateMaterial(original, Path.GetDirectoryName(preferredPath));
            if (_shader) material.shader = _shader;
            if (material != original) AssetDatabase.CreateAsset(material, preferredPath);
            return material;
        }
    }
}
