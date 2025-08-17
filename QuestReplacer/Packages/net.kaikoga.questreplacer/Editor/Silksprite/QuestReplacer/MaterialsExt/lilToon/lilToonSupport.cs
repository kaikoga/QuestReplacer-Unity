using System;
using System.Reflection;
using lilToon;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.MaterialsExt.lilToon
{
    public static class lilToonSupport
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

        static lilToonInspector InitializeInspector(Material material)
        {
            var props = MaterialEditor.GetMaterialProperties(new Object[] { material });
            var setPropertiesMethod = typeof(lilToonInspector).GetMethod("SetProperties", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
            var checkShaderTypeMethod = typeof(lilToonInspector).GetMethod("CheckShaderType", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
            var loadCustomPropertiesMethod = typeof(lilToonInspector).GetMethod("LoadCustomProperties", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
            var inspector = new lilToonInspector();
            setPropertiesMethod!.Invoke(inspector, new object[] { props });
            checkShaderTypeMethod!.Invoke(inspector, new object[] { material });
            loadCustomPropertiesMethod!.Invoke(inspector, new object[] { props, material });
            return inspector;
        }

        public static Texture2D AutoBakeMainTexture(Material material)
        {
            return Wrap(null, () =>
            {
                var inspector = InitializeInspector(material);
                var autoBakeMainTextureMethod = typeof(lilToonInspector).GetMethod("AutoBakeMainTexture", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
                return autoBakeMainTextureMethod!.Invoke(inspector, new object[] { material }) as Texture2D;
            });
        }
        public static Texture2D AutoBakeShadowTexture(Material material, Texture2D bakedMainTex, int shadowType = 0, bool shouldShowDialog = true)
        {
            return Wrap(null, () =>
            {
                var inspector = InitializeInspector(material);
                var autoBakeShadowTextureMethod = typeof(lilToonInspector).GetMethod("AutoBakeShadowTexture", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
                return autoBakeShadowTextureMethod!.Invoke(inspector, new object[] { material, bakedMainTex, shadowType, shouldShowDialog }) as Texture2D;
            });
        }
        public static Texture2D AutoBakeMatCap(Material material)
        {
            return Wrap(null, () =>
            {
                var inspector = InitializeInspector(material);
                var autoBakeMatCapMethod = typeof(lilToonInspector).GetMethod("AutoBakeMatCap", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
                return autoBakeMatCapMethod!.Invoke(inspector, new object[] { material }) as Texture2D;
            });
        }
    }
}