using System;
using System.Diagnostics.CodeAnalysis;
using Silksprite.AdLib.Material.Access.Impl;
using Silksprite.AdLib.Material.Impl;
using Silksprite.QuestReplacer.Assets;
using UnityEngine;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class lilToonToVRMMaterialDuplicator : ISingleAssetDuplicator<Material>
    {
        static bool _lilToonSupportErrorReported;

        static T Wrap<T>(T original, Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                if (_lilToonSupportErrorReported) return original;

                _lilToonSupportErrorReported = true;
                Debug.LogException(e);
                Debug.LogError("Something was wrong in lilToon support of QuestReplacer.");
                return original;
            }
        }

        int ISingleAssetDuplicator<Material>.Priority => 10000;

        bool ISingleAssetDuplicator<Material>.IsTarget(Material original)
        {
            var originalShaderName = original.shader.name;
            return originalShaderName.Contains("lilToon") || originalShaderName.StartsWith("Hidden/lts");
        }

        Material ISingleAssetDuplicator<Material>.Duplicate(Material original, string bakedAssetDirectoryPath)
        {
            return Wrap(original, () =>
            {
                var mtoonMaterial = new Material(Shaders.VrmMToon);
                var lilToonAccess = new LilToonMaterialAccess(original.ToMaterialAccess());
                var mtoonAccess = new MToonMaterialAccess(mtoonMaterial.ToMaterialAccess());

                CopyBasicProperties(lilToonAccess, mtoonAccess);
                CopyRenderingProperties(lilToonAccess, mtoonAccess);
                CopyShadowProperties(lilToonAccess, mtoonAccess);
                CopyEmissionProperties(lilToonAccess, mtoonAccess);
                CopyRimProperties(lilToonAccess, mtoonAccess);
                CopyMatCapProperties(lilToonAccess, mtoonAccess);
                CopyOutlineProperties(lilToonAccess, mtoonAccess);
                CopyUvAnimationProperties(lilToonAccess, mtoonAccess);

                return mtoonMaterial;
            });
        }

        static void CopyBasicProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            mtoon.Color.Value = new Color(
                Mathf.Clamp01(lilToon.MainColor.Value.r),
                Mathf.Clamp01(lilToon.MainColor.Value.g),
                Mathf.Clamp01(lilToon.MainColor.Value.b),
                Mathf.Clamp01(lilToon.MainColor.Value.a));

            mtoon.LightColorAttenuation.Value = 0.0f;
            mtoon.IndirectLightIntensity.Value = 0.0f;
            mtoon.MToonVersion.Value = 35.0f;
            mtoon.DebugMode.Value = 0.0f;
            switch (lilToon.Cull.Value)
            {
                case LilToonEnums.CullMode.Off:
                    mtoon.CullMode.Value = MToonEnums.CullMode.Off;
                    break;
                case LilToonEnums.CullMode.Front:
                    mtoon.CullMode.Value = MToonEnums.CullMode.Front;
                    break;
                case LilToonEnums.CullMode.Back:
                    mtoon.CullMode.Value = MToonEnums.CullMode.Back;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var bakedMainTex = AutoBakeMainTexture(lilToon);
            mtoon.MainTex.Value = bakedMainTex;

            mtoon.MainTex.TextureScale = lilToon.MainTex.TextureScale;
            mtoon.MainTex.TextureOffset = lilToon.MainTex.TextureOffset;
        }

        static void CopyRenderingProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.IsCutout)
            {
                mtoon.BlendMode.Value = MToonEnums.RenderMode.Cutout;
                mtoon.Cutoff.Value = lilToon.Cutoff.Value;
                mtoon.SrcBlend.Value = (float)UnityEngine.Rendering.BlendMode.One;
                mtoon.DstBlend.Value = (float)UnityEngine.Rendering.BlendMode.Zero;
                mtoon.ZWrite.Value = 1.0f;
                mtoon.AlphaToMask.Value = 1.0f;
                mtoon.ALPHATEST_ON.Value = true;
                mtoon.RenderQueue.Value = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
            else if (lilToon.IsTransparent && !lilToon.ZWrite.Value)
            {
                mtoon.BlendMode.Value = MToonEnums.RenderMode.Transparent;
                mtoon.SrcBlend.Value = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                mtoon.DstBlend.Value = (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                mtoon.ZWrite.Value = 0.0f;
                mtoon.AlphaToMask.Value = 0.0f;
                mtoon.ALPHABLEND_ON.Value = true;
                mtoon.RenderQueue.Value = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else if (lilToon.IsTransparent && lilToon.ZWrite.Value)
            {
                mtoon.BlendMode.Value = MToonEnums.RenderMode.TransparentWithZWrite;
                mtoon.SrcBlend.Value = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                mtoon.DstBlend.Value = (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                mtoon.ZWrite.Value = 1.0f;
                mtoon.AlphaToMask.Value = 0.0f;
                mtoon.ALPHABLEND_ON.Value = true;
                mtoon.RenderQueue.Value = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mtoon.BlendMode.Value = MToonEnums.RenderMode.Opaque;
                mtoon.SrcBlend.Value = (float)UnityEngine.Rendering.BlendMode.One;
                mtoon.DstBlend.Value = (float)UnityEngine.Rendering.BlendMode.Zero;
                mtoon.ZWrite.Value = 1.0f;
                mtoon.AlphaToMask.Value = 0.0f;
                mtoon.RenderQueue.Value = -1;
            }
        }

        static void CopyShadowProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.UseShadow.Value)
            {
                var shadeShift = Mathf.Clamp01(lilToon.ShadowBorder.Value - lilToon.ShadowBlur.Value * 0.5f) * 2.0f - 1.0f;
                var shadeToony = shadeShift == 1.0f 
                    ? 1.0f
                    : (2.0f - Mathf.Clamp01(lilToon.ShadowBorder.Value + lilToon.ShadowBlur.Value * 0.5f) * 2.0f) / (1.0f - shadeShift);

                if (lilToon.ShadowStrengthMask.Value || lilToon.ShadowMainStrength.Value != 0.0f)
                {
                    var bakedShadowTex = AutoBakeShadowTexture(lilToon, mtoon.MainTex.Value);
                    mtoon.ShadeColor.Value = Color.white;
                    mtoon.ShadeTexture.Value = bakedShadowTex;
                }
                else
                {
                    var shadeColorStrength = new Color(
                        1.0f - (1.0f - lilToon.ShadowColor.Value.r) * lilToon.ShadowStrength.Value,
                        1.0f - (1.0f - lilToon.ShadowColor.Value.g) * lilToon.ShadowStrength.Value,
                        1.0f - (1.0f - lilToon.ShadowColor.Value.b) * lilToon.ShadowStrength.Value,
                        1.0f);
                    mtoon.ShadeColor.Value = shadeColorStrength;

                    mtoon.ShadeTexture.Value = lilToon.ShadowColorTex.Value ? lilToon.ShadowColorTex.Value : mtoon.MainTex.Value;
                }

                mtoon.ReceiveShadowRate.Value = 1.0f;
                mtoon.ReceiveShadowTexture.Value = null;
                mtoon.ShadingGradeRate.Value = 1.0f;
                mtoon.ShadingGradeTexture.Value = lilToon.ShadowBorderMask.Value;
                mtoon.ShadeShift.Value = shadeShift;
                mtoon.ShadeToony.Value = shadeToony;
            }
            else
            {
                mtoon.ShadeColor.Value = Color.white;
                mtoon.ShadeTexture.Value = mtoon.MainTex.Value;
            }
        }

        static void CopyEmissionProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.UseEmission.Value && lilToon.EmissionMap.Value)
            {
                mtoon.EmissionColor.Value = lilToon.EmissionColor.Value;
                mtoon.EmissionMap.Value = lilToon.EmissionMap.Value;
            }
        }

        static void CopyRimProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.UseRim.Value)
            {
                mtoon.RimColor.Value = lilToon.RimColor.Value;
                mtoon.RimTexture.Value = lilToon.RimColorTex.Value;
                mtoon.RimLightingMix.Value = lilToon.RimEnableLighting.Value;

                var rimFP = lilToon.RimFresnelPower.Value / Mathf.Max(0.001f, lilToon.RimBlur.Value);
                var rimLift = Mathf.Pow(1.0f - lilToon.RimBorder.Value, lilToon.RimFresnelPower.Value) * (1.0f - lilToon.RimBlur.Value);
                mtoon.RimFresnelPower.Value = rimFP;
                mtoon.RimLift.Value = rimLift;
            }
            else
            {
                mtoon.RimColor.Value = Color.black;
            }
        }

        static void CopyMatCapProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.UseMatCap.Value && lilToon.MatCapBlendMode.Value != LilToonEnums.BlendMode.Mul && lilToon.MatCapTex.Value != null)
            {
                var bakedMatCap = AutoBakeMatCap(lilToon);
                mtoon.SphereAdd.Value = bakedMatCap;
            }
        }

        static void CopyOutlineProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            if (lilToon.IsOutl)
            {
                mtoon.OutlineWidthTexture.Value = lilToon.OutlineWidthMask.Value;
                mtoon.OutlineWidth.Value = lilToon.OutlineWidth.Value;
                mtoon.OutlineColor.Value = lilToon.OutlineColor.Value;
                mtoon.OutlineLightingMix.Value = 1.0f;
                mtoon.OutlineWidthMode.Value = MToonEnums.OutlineWidthMode.WorldCoordinates;
                
                mtoon.OutlineColorMode.Value = 1.0f;
                mtoon.OutlineCullMode.Value = 1.0f;
                
                mtoon.MTOON_OUTLINE_WIDTH_WORLD.Value = true;
                mtoon.MTOON_OUTLINE_COLOR_MIXED.Value = true;
            }
        }

        static void CopyUvAnimationProperties(LilToonMaterialAccess lilToon, MToonMaterialAccess mtoon)
        {
            var scrollRotate = lilToon.MainTexScrollRotate.Value;
            mtoon.UvAnimScrollX.Value = scrollRotate.x;
            mtoon.UvAnimScrollY.Value = scrollRotate.y;
            mtoon.UvAnimRotation.Value = scrollRotate.w / Mathf.PI * 0.5f;
        }

        static Texture2D AutoBakeMainTexture(LilToonMaterialAccess lilToon)
        {
            return lilToonSupport.AutoBakeMainTexture(lilToon.Target);
        }

        static Texture2D AutoBakeShadowTexture(LilToonMaterialAccess lilToon, Texture2D mainTex, int shadowType = 0, bool shouldShowDialog = true)
        {
            return lilToonSupport.AutoBakeShadowTexture(lilToon.Target, mainTex, shadowType, shouldShowDialog);
        }

        static Texture2D AutoBakeMatCap(LilToonMaterialAccess lilToon)
        {
            return lilToonSupport.AutoBakeMatCap(lilToon.Target);
        }
    }
}
