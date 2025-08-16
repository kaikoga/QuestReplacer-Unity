using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if QUESTREPLACER_LILTOON
using lilToon;
using UnityEditor;
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
                var materialProperties = MaterialEditor.GetMaterialProperties(new Object[] { original });
                var inspector = new lilToonInspector();
                inspector.InitializeInspector(materialProperties, original);
                return inspector.CreateMToonMaterialVolatile(original);
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