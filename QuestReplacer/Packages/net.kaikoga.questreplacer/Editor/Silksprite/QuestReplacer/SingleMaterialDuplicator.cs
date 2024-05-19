using Silksprite.QuestReplacer.External;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public interface ISingleMaterialDuplicator
    {
        bool IsTarget(Material original);
        Material Duplicate(Material original, string preferredPath);
    }

    public class SingleMaterialDuplicator : ISingleMaterialDuplicator
    {
        readonly string _pattern;
        readonly Shader _shader;

        public SingleMaterialDuplicator(string pattern, Shader shader)
        {
            _pattern = pattern;
            _shader = shader;
        }

        public bool IsTarget(Material original) => original.shader.name.Contains(_pattern);

        public Material Duplicate(Material original, string preferredPath)
        {
            // cannot be material variant because shader is different
            var material = new Material(original)
            {
                shader = _shader
            };
            AssetDatabase.CreateAsset(material, preferredPath);
            return material;
        }
    }

    public class lilToonMaterialDuplicator : ISingleMaterialDuplicator
    {
        readonly Shader _shader;

        public lilToonMaterialDuplicator(Shader shader = null)
        {
            _shader = shader;
        }

        public bool IsTarget(Material original)
        {
            return false; // disable because lilToonSupport is not working
            // var originalShaderName = original.shader.name;
            // return originalShaderName.Contains("lilToon") || originalShaderName.StartsWith("Hidden/lts");
        }

        public Material Duplicate(Material original, string preferredPath)
        {
            var material = lilToonSupport.lilDuplicateMaterial(original);
            if (_shader) material.shader = _shader;
            return material;
        }
    }
}