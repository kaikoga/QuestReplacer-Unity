using System;
using UnityEngine;

#if QUESTREPLACER_VRCQUESTTOOLS
using System.Reflection;
using KRT.VRCQuestTools;
using KRT.VRCQuestTools.Models;
#endif

namespace Silksprite.QuestReplacer.MaterialsExt.Support
{
    public static class VRCQuestToolsSupport
    {

#if QUESTREPLACER_VRCQUESTTOOLS

        static bool _vrcQuestToolsSupportErrorReported;

        public static Material ConvertSingleMaterial(Material original, string bakedAssetDirectoryPath)
        {
            try
            {
                var converted = original;
                var assembly = typeof(VRCQuestTools).Assembly;
                
                var materialWrapperBuilderType = assembly.GetType("KRT.VRCQuestTools.Models.Unity.MaterialWrapperBuilder");
                var avatarConverterType = assembly.GetType("KRT.VRCQuestTools.Models.VRChat.AvatarConverter");
                var asyncCallbackRequestType = assembly.GetType("KRT.VRCQuestTools.Utils.AsyncCallbackRequest");

                var generateConvertedMaterialMethod = avatarConverterType.GetMethod("GenerateConvertedMaterial", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                var buildMethod = materialWrapperBuilderType.GetMethod("Build", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                var waitForCompletionMethod = asyncCallbackRequestType.GetMethod("WaitForCompletion", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                
                var materialWrapperBuilder = Activator.CreateInstance(materialWrapperBuilderType, true);
                var avatarConverter = Activator.CreateInstance(avatarConverterType, BindingFlags.Instance | BindingFlags.NonPublic, null, new [] { materialWrapperBuilder }, null);

                var materialBase = buildMethod!.Invoke(materialWrapperBuilder, new object[] { original });
                var convertSettings = new ToonStandardConvertSettings
                {
                    generateQuestTextures = false,
                    maxTextureSize = TextureSizeLimit.Max1024x1024,
                    mobileTextureFormat = MobileTextureFormat.ASTC_6x6,
                    fallbackShadowRamp = null
                };
                convertSettings.LoadDefaultAssets();
                var asyncCallbackRequest = generateConvertedMaterialMethod!.Invoke(avatarConverter, new []
                {
                    materialBase,
                    convertSettings,
                    true,
                    bakedAssetDirectoryPath,
                    (Action<Material>)(material => converted = material)
                });
                waitForCompletionMethod!.Invoke(asyncCallbackRequest, new object[]{ });
                return converted;
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

#else

        public static Material ConvertSingleMaterial(Material original) => original;

#endif
        
    }
}