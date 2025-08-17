using System;
using System.Reflection;
using KRT.VRCQuestTools.Models;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.VRCQuestTools
{
    public static class VRCQuestToolsSupport
    {
        public static Material ConvertSingleMaterial(Material original, string bakedAssetDirectoryPath)
        {
            var converted = original;
            var assembly = typeof(KRT.VRCQuestTools.VRCQuestTools).Assembly;
            
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
    }
}