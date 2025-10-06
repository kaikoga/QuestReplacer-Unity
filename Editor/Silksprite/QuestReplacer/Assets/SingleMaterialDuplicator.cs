using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class SingleMaterialDuplicator : ISingleAssetDuplicator<Material>
    {
        readonly string _pattern;
        readonly Shader _shader;

        public SingleMaterialDuplicator(string pattern, Shader shader)
        {
            _pattern = pattern;
            _shader = shader;
        }


        int ISingleAssetDuplicator<Material>.Priority => _pattern.Length;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original) => original.shader.name.Contains(_pattern);
        bool ISingleAssetDuplicator<Material>.TryDuplicate(Material original, string bakedAssetDirectoryPath, out Material result)
        {
            // cannot be material variant because shader is different
            result = new Material(original)
            {
                shader = _shader
            };
            return true;
        }
    }
}