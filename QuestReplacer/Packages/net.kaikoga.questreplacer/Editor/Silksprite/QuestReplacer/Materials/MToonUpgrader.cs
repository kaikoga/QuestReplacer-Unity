using Silksprite.AdLib.Material.Access.Impl;
using Silksprite.AdLib.Material.Impl;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class MToonUpgrader : ISingleMaterialDuplicator
    {
        public bool IsTarget(Material original) => original.shader.name == "VRM/MToon";

        public Material Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            var mtoon = new MToonMaterialAccess(original.ToMaterialAccess());
            var material = new Material(Shaders.VrmMToon10);
            var mtoon10 = new MToon10MaterialAccess(material.ToMaterialAccess());
            CopyMainTex(mtoon, mtoon10);
            return material;
        }

        void CopyMainTex(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.MainTex.Value = mtoon.MainTex.Value;
        }
    }
}