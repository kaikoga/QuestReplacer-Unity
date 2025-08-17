using System;
using Silksprite.QuestReplacer.Materials;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public class VRCQuestToolsMaterialDuplicator : ISingleMaterialDuplicator
    {
        static bool _vrcQuestToolsSupportErrorReported;

        static T Wrap<T>(T original, Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                if (_vrcQuestToolsSupportErrorReported) return original;

                _vrcQuestToolsSupportErrorReported = true;
                Debug.LogException(e);
                Debug.LogError("Something was wrong in VRCQuestTools support of QuestReplacer.");
                return original;
            }
        }

        bool ISingleMaterialDuplicator.IsTarget(Material original) => true;

        Material ISingleMaterialDuplicator.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            return Wrap(original, () =>
            {
                var material = VRCQuestToolsSupport.ConvertSingleMaterial(original, bakedAssetDirectoryPath);
                return material;
            });
        }
    }
}
