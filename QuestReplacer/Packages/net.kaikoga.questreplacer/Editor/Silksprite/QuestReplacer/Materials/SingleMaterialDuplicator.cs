using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class SingleMaterialDuplicator : ISingleMaterialDuplicator
    {
        readonly string _pattern;
        readonly Shader _shader;

        public SingleMaterialDuplicator(string pattern, Shader shader)
        {
            _pattern = pattern;
            _shader = shader;
        }

        bool ISingleMaterialDuplicator.IsTarget(Material original) => original.shader.name.Contains(_pattern);

        public Material Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            // cannot be material variant because shader is different
            var material = new Material(original)
            {
                shader = _shader
            };
            return material;
        }
    }
}