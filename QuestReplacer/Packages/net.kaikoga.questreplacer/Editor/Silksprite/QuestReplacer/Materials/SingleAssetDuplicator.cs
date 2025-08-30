using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class SingleAssetDuplicator : ISingleAssetDuplicator<Material>
    {
        readonly string _pattern;
        readonly Shader _shader;

        public SingleAssetDuplicator(string pattern, Shader shader)
        {
            _pattern = pattern;
            _shader = shader;
        }


        int ISingleAssetDuplicator<Material>.Priority => _pattern.Length;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original) => original.shader.name.Contains(_pattern);

        Material ISingleAssetDuplicator<Material>.Duplicate(Material original, string bakedAssetDirectoryPath)
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