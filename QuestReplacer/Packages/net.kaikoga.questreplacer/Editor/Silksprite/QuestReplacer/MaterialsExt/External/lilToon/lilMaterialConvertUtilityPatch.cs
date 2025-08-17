using UnityEditor;
using UnityEngine;

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
    }
}
