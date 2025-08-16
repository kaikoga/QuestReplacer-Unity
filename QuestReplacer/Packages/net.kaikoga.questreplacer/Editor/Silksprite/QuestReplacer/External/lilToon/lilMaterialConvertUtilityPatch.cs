using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToon
{
    public partial class lilToonInspector
    {
        public void InitializeInspector(MaterialProperty[] props, Material material)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            // m_MaterialEditor = materialEditor;
            // lilShaderManager.InitializeShaders();
            // lilToonSetting.InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            SetProperties(props);

            //------------------------------------------------------------------------------------------------------------------------------
            // Check Shader Type
            CheckShaderType(material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Custom Properties
            LoadCustomProperties(props, material);
        }

        public Material CreateMToonMaterialVolatile(Material material)
        {
            var mtoonMaterial = new Material(mtoon);

            // string matPath = AssetDatabase.GetAssetPath(material);
            // if(!string.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_mtoon", "mat");
            // else                                matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            // if(!string.IsNullOrEmpty(matPath))  AssetDatabase.CreateAsset(mtoonMaterial, FileUtil.GetProjectRelativePath(matPath));

            mtoonMaterial.SetColor("_Color",                    new Color(Mathf.Clamp01(mainColor.colorValue.r), Mathf.Clamp01(mainColor.colorValue.g), Mathf.Clamp01(mainColor.colorValue.b), Mathf.Clamp01(mainColor.colorValue.a)));
            mtoonMaterial.SetFloat("_LightColorAttenuation",    0.0f);
            mtoonMaterial.SetFloat("_IndirectLightIntensity",   0.0f);

            mtoonMaterial.SetFloat("_UvAnimScrollX",            mainTex_ScrollRotate.vectorValue.x);
            mtoonMaterial.SetFloat("_UvAnimScrollY",            mainTex_ScrollRotate.vectorValue.y);
            mtoonMaterial.SetFloat("_UvAnimRotation",           mainTex_ScrollRotate.vectorValue.w / Mathf.PI * 0.5f);
            mtoonMaterial.SetFloat("_MToonVersion",             35.0f);
            mtoonMaterial.SetFloat("_DebugMode",                0.0f);
            mtoonMaterial.SetFloat("_CullMode",                 cull.floatValue);

            var bakedMainTex = AutoBakeMainTexture(material);
            mtoonMaterial.SetTexture("_MainTex", bakedMainTex);

            var mainScale = material.GetTextureScale(mainTex.name);
            var mainOffset = material.GetTextureOffset(mainTex.name);
            mtoonMaterial.SetTextureScale(mainTex.name, mainScale);
            mtoonMaterial.SetTextureOffset(mainTex.name, mainOffset);

            if(useBumpMap.floatValue == 1.0f && bumpMap.textureValue != null)
            {
                mtoonMaterial.SetFloat("_BumpScale", bumpScale.floatValue);
                mtoonMaterial.SetTexture("_BumpMap", bumpMap.textureValue);
                mtoonMaterial.EnableKeyword("_NORMALMAP");
            }

            if(useShadow.floatValue == 1.0f)
            {
                float shadeShift = (Mathf.Clamp01(shadowBorder.floatValue - (shadowBlur.floatValue * 0.5f)) * 2.0f) - 1.0f;
                float shadeToony = shadeShift == 1.0f ? 1.0f : (2.0f - (Mathf.Clamp01(shadowBorder.floatValue + (shadowBlur.floatValue * 0.5f)) * 2.0f)) / (1.0f - shadeShift);
                if(shadowStrengthMask.textureValue != null || shadowMainStrength.floatValue != 0.0f)
                {
                    var bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex);
                    mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                    mtoonMaterial.SetTexture("_ShadeTexture",           bakedShadowTex);
                }
                else
                {
                    var shadeColorStrength = new Color(
                        1.0f - ((1.0f - shadowColor.colorValue.r) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.g) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.b) * shadowStrength.floatValue),
                        1.0f
                    );
                    mtoonMaterial.SetColor("_ShadeColor",               shadeColorStrength);
                    if(shadowColorTex.textureValue != null)
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           shadowColorTex.textureValue);
                    }
                    else
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
                    }
                }
                mtoonMaterial.SetFloat("_ReceiveShadowRate",        1.0f);
                mtoonMaterial.SetTexture("_ReceiveShadowTexture",   null);
                mtoonMaterial.SetFloat("_ShadingGradeRate",         1.0f);
                mtoonMaterial.SetTexture("_ShadingGradeTexture",    shadowBorderMask.textureValue);
                mtoonMaterial.SetFloat("_ShadeShift",               shadeShift);
                mtoonMaterial.SetFloat("_ShadeToony",               shadeToony);
            }
            else
            {
                mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
            }

            if(useEmission.floatValue == 1.0f && emissionMap.textureValue != null)
            {
                mtoonMaterial.SetColor("_EmissionColor",            emissionColor.colorValue);
                mtoonMaterial.SetTexture("_EmissionMap",            emissionMap.textureValue);
            }

            if(useRim.floatValue == 1.0f)
            {
                mtoonMaterial.SetColor("_RimColor",                 rimColor.colorValue);
                mtoonMaterial.SetTexture("_RimTexture",             rimColorTex.textureValue);
                mtoonMaterial.SetFloat("_RimLightingMix",           rimEnableLighting.floatValue);

                float rimFP = rimFresnelPower.floatValue / Mathf.Max(0.001f, rimBlur.floatValue);
                float rimLift = Mathf.Pow(1.0f - rimBorder.floatValue, rimFresnelPower.floatValue) * (1.0f - rimBlur.floatValue);
                mtoonMaterial.SetFloat("_RimFresnelPower",          rimFP);
                mtoonMaterial.SetFloat("_RimLift",                  rimLift);
            }
            else
            {
                mtoonMaterial.SetColor("_RimColor",                 Color.black);
            }

            if(useMatCap.floatValue == 1.0f && matcapBlendMode.floatValue != 3.0f && matcapTex.textureValue != null)
            {
                var bakedMatCap = AutoBakeMatCap(material);
                mtoonMaterial.SetTexture("_SphereAdd", bakedMatCap);
            }

            if(isOutl)
            {
                mtoonMaterial.SetTexture("_OutlineWidthTexture",    outlineWidthMask.textureValue);
                mtoonMaterial.SetFloat("_OutlineWidth",             outlineWidth.floatValue);
                mtoonMaterial.SetColor("_OutlineColor",             outlineColor.colorValue);
                mtoonMaterial.SetFloat("_OutlineLightingMix",       1.0f);
                mtoonMaterial.SetFloat("_OutlineWidthMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineColorMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineCullMode",          1.0f);
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_WIDTH_WORLD");
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_COLOR_MIXED");
            }

            if(isCutout)
            {
                mtoonMaterial.SetFloat("_Cutoff", cutoff.floatValue);
                mtoonMaterial.SetFloat("_BlendMode", 1.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 1.0f);
                mtoonMaterial.EnableKeyword("_ALPHATEST_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.AlphaTest;
            }
            else if(isTransparent && zwrite.floatValue == 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 2.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 0.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.Transparent;
            }
            else if(isTransparent && zwrite.floatValue != 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 3.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                mtoonMaterial.SetFloat("_BlendMode", 0.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "Opaque");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.renderQueue = -1;
            }
            return mtoonMaterial;
        }

    }
}
