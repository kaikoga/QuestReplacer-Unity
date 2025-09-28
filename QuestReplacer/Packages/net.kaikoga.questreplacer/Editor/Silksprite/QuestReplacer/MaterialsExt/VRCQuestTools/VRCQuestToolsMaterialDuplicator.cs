using System;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public class VRCQuestToolsMaterialDuplicator : ISingleAssetDuplicator<Material>
    {
        static bool _vrcQuestToolsSupportErrorReported;

        static bool Wrap<T>(T original, Func<T> func, out T result)
        {
            try
            {
                result = func();
                return true;
            }
            catch (Exception e)
            {
                if (!_vrcQuestToolsSupportErrorReported)
                {
                    _vrcQuestToolsSupportErrorReported = true;
                    Debug.LogException(e);
                    Debug.LogError("Something was wrong in VRCQuestTools support of QuestReplacer.");
                }
                result = original;
                return false;
            }
        }

        int ISingleAssetDuplicator<Material>.Priority => 0;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original) => true;

        bool ISingleAssetDuplicator<Material>.TryDuplicate(Material original, string bakedAssetDirectoryPath, out Material result)
        {
            return Wrap(original, () =>
            {
                var material = VRCQuestToolsSupport.ConvertSingleMaterial(original, bakedAssetDirectoryPath);
                return material;
            }, out result);
        }
    }
}
