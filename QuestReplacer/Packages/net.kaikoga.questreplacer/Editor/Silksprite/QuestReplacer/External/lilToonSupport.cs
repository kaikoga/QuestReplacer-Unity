using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if QUESTREPLACER_LILTOON
using lilToon;
#endif

namespace Silksprite.QuestReplacer.External
{
    public static class lilToonSupport
    {

#if QUESTREPLACER_LILTOON

        static bool _lilToonSupportErrorReported;

        public static Material lilDuplicateMaterial(Material original)
        {
            try
            {
                var lilToonInspectorType = typeof(lilToonInspector);
                var createMToonMaterial = lilToonInspectorType.GetMethod("CreateMToonMaterial", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);

                var lilToonInspector = new lilToonInspector();
                var props = MaterialEditor.GetMaterialProperties(new Object[] { original });
                try
                {
                    lilToonInspector.DrawAllGUI(null, props, original);                    
                }
                catch (Exception)
                {
                    // this should fail because MaterialEditor argument is null
                }

                // FIXME
                return createMToonMaterial.Invoke(lilToonInspector, new object[] { original }) as Material;
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

#else

        public static Material lilDuplicateMaterial(Material original) => original;

#endif
        
    }
}