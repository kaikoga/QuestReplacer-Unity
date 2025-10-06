using System;
using Silksprite.AdLib.Material.Access.Impl;
using Silksprite.AdLib.Material.Impl;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

#if QUESTREPLACER_VRMSHADERS
using VRMShaders.VRM10.MToon10.Runtime;
#elif QUESTREPLACER_UNIVRM_WITH_SHADERS
using VRM10.MToon10;
#endif

namespace Silksprite.QuestReplacer.MaterialsExt.VRMShaders
{
    public class MToonUpgrader : ISingleAssetDuplicator<Material>
    {
        int ISingleAssetDuplicator<Material>.Priority => 20000;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original) => original.shader.name == "VRM/MToon";

        bool ISingleAssetDuplicator<Material>.TryDuplicate(Material original, string bakedAssetDirectoryPath, out Material result)
        {
            var mtoon = new MToonMaterialAccess(original.ToMaterialAccess());
            result = new Material(Shaders.VrmMToon10);
            var mtoon10 = new MToon10MaterialAccess(result.ToMaterialAccess());
            CopyRendering(mtoon, mtoon10);
            CopyMainTex(mtoon, mtoon10);
            CopyEmissionAndMatcap(mtoon, mtoon10);
            CopyRimLighting(mtoon, mtoon10);
            CopyOutline(mtoon, mtoon10);
            CopyUvAnimation(mtoon, mtoon10);
            new MToonValidator(result).Validate();
            return true;
        }

        static void CopyRendering(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            switch (mtoon.BlendMode.Value)
            {
                case MToonEnums.RenderMode.Opaque:
                    mtoon10.AlphaMode.Value = MToon10Enums.MToon10AlphaMode.Opaque;
                    mtoon10.TransparentWithZWrite.Value = MToon10Enums.MToon10TransparentWithZWriteMode.Off;
                    break;
                case MToonEnums.RenderMode.Cutout:
                    mtoon10.AlphaMode.Value = MToon10Enums.MToon10AlphaMode.Cutout;
                    mtoon10.TransparentWithZWrite.Value = MToon10Enums.MToon10TransparentWithZWriteMode.Off;
                    break;
                case MToonEnums.RenderMode.Transparent:
                    mtoon10.AlphaMode.Value = MToon10Enums.MToon10AlphaMode.Transparent;
                    mtoon10.TransparentWithZWrite.Value = MToon10Enums.MToon10TransparentWithZWriteMode.Off;
                    break;
                case MToonEnums.RenderMode.TransparentWithZWrite:
                    mtoon10.AlphaMode.Value = MToon10Enums.MToon10AlphaMode.Transparent;
                    mtoon10.TransparentWithZWrite.Value = MToon10Enums.MToon10TransparentWithZWriteMode.On;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            mtoon10.AlphaCutoff.Value = mtoon.Cutoff.Value;
            switch (mtoon.CullMode.Value)
            {
                case MToonEnums.CullMode.Off:
                    mtoon10.DoubleSided.Value = MToon10Enums.MToon10DoubleSidedMode.On;
                    break;
                case MToonEnums.CullMode.Front:
                    mtoon10.DoubleSided.Value = MToon10Enums.MToon10DoubleSidedMode.On;
                    break;
                case MToonEnums.CullMode.Back:
                    mtoon10.DoubleSided.Value = MToon10Enums.MToon10DoubleSidedMode.Off;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            mtoon10.RenderQueueOffsetNumber.Value = 0f; // TODO
        }

        static void CopyMainTex(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.MainTex.Value = mtoon.MainTex.Value;
            mtoon10.Color.Value = mtoon.Color.Value;
            mtoon10.ShadeTex.Value = mtoon.ShadeTexture.Value ?? mtoon.MainTex.Value;
            mtoon10.ShadeColor.Value = mtoon.ShadeColor.Value;
            mtoon10.BumpMap.Value = mtoon.BumpMap.Value;
            mtoon10.BumpScale.Value = mtoon.BumpScale.Value;
            
            mtoon10.ShadingToonyFactor.Value = MToon10Migrator.MigrateToShadingToony(
                mtoon.ShadeToony.Value, 
                mtoon.ShadeShift.Value);
            mtoon10.ShadingShiftFactor.Value = MToon10Migrator.MigrateToShadingShift(
                mtoon.ShadeToony.Value, 
                mtoon.ShadeShift.Value);
            
            mtoon10.GiEqualization.Value = MToon10Migrator.MigrateToGiEqualization(
                mtoon.IndirectLightIntensity.Value);
        }

        static void CopyEmissionAndMatcap(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.EmissionMap.Value = mtoon.EmissionMap.Value;
            mtoon10.EmissionColor.Value = mtoon.EmissionColor.Value;
            
            mtoon10.MatcapTex.Value = mtoon.SphereAdd.Value;
        }

        static void CopyRimLighting(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.RimColor.Value = mtoon.RimColor.Value;
            mtoon10.RimFresnelPower.Value = mtoon.RimFresnelPower.Value;
            mtoon10.RimLift.Value = mtoon.RimLift.Value;
            mtoon10.RimTex.Value = mtoon.RimTexture.Value;
            
            // NOTE: DESTRUCTIVE MIGRATION
            // Rim Lighting behaviour will be merged with MatCap in VRM 1.0.
            // So, RimLightingMixFactor set to 1.0, because it is safe appearance.
            mtoon10.RimLightingMix.Value = 1.0f;
        }

        static void CopyOutline(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.OutlineWidthMode.Value = mtoon.OutlineWidthMode.Value switch
            {

                MToonEnums.OutlineWidthMode.None => MToon10Enums.MToon10OutlineMode.None,
                MToonEnums.OutlineWidthMode.WorldCoordinates => MToon10Enums.MToon10OutlineMode.World,
                MToonEnums.OutlineWidthMode.ScreenCoordinates => MToon10Enums.MToon10OutlineMode.Screen,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            switch (mtoon.OutlineWidthMode.Value)
            {
                case MToonEnums.OutlineWidthMode.None:
                    mtoon10.OutlineWidth.Value = 0f;
                    break;
                case MToonEnums.OutlineWidthMode.WorldCoordinates:
                    mtoon10.OutlineWidth.Value = mtoon.OutlineWidth.Value * 0.01f;
                    break;
                case MToonEnums.OutlineWidthMode.ScreenCoordinates:
                    // NOTE: 従来は、縦幅の半分を 100% としたときの % の値だった。
                    //       1.0 では縦幅を 1 としたときの値とするので、 1/200 する。
                    mtoon10.OutlineWidth.Value = mtoon.OutlineWidth.Value * 0.005f;
                    break;
            }
            
            mtoon10.OutlineColor.Value = mtoon.OutlineColor.Value;
            mtoon10.OutlineWidthTex.Value = mtoon.OutlineWidthTexture.Value;
            mtoon10.OutlineLightingMix.Value = mtoon.OutlineLightingMix.Value;
        }

        static void CopyUvAnimation(MToonMaterialAccess mtoon, MToon10MaterialAccess mtoon10)
        {
            mtoon10.UvAnimMaskTex.Value = mtoon.UvAnimMaskTexture.Value;
            mtoon10.UvAnimScrollXSpeed.Value = mtoon.UvAnimScrollX.Value;
            mtoon10.UvAnimScrollYSpeed.Value = mtoon.UvAnimScrollY.Value * -1f;
            mtoon10.UvAnimRotationSpeed.Value = mtoon.UvAnimRotation.Value;
        }
    }
}