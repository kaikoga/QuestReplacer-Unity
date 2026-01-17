using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;

namespace Silksprite.QuestReplacer
{
    public static class GizmoIconsDisabler
    {
        static readonly Type TargetComponentBase = typeof(QuestReplacer);

        static readonly Type AnnotationUtility = Assembly.GetAssembly(typeof(Editor))
            ?.GetType("UnityEditor.AnnotationUtility");

        static readonly MethodInfo GetAnnotations = AnnotationUtility
            ?.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);

        static readonly MethodInfo SetIconEnabled = AnnotationUtility
            ?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

        static readonly Type Annotation = Assembly.GetAssembly(typeof(Editor))
            ?.GetType("UnityEditor.Annotation");

        static readonly FieldInfo ScriptClass = Annotation
            ?.GetField("scriptClass", BindingFlags.Instance | BindingFlags.Public);

        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            var a = Type.GetType("UnityEditor.AnnotationUtility");
            _ = InitializeOnLoadAsync();
        }
        
        static async Task InitializeOnLoadAsync()
        {
            // NOTE: Unity 2021 is supported 
            await DisableGizmoIcons(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => TargetComponentBase.IsAssignableFrom(type) && !type.IsAbstract)
                .ToArray());
        }
        
        const int MonoBehaviourClassId = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
        static async Task DisableGizmoIcons(Type[] types)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (SetIconEnabled == null)
                {
                    return;
                }
                    
                var annotations = ((Array)GetAnnotations.Invoke(null, new object[] { })).Cast<object>();
                if (annotations.All(annotation => (string)ScriptClass.GetValue(annotation) != types[0].Name))
                {
                    continue;
                }

                foreach (var type in types)
                {
                    SetIconEnabled.Invoke(null, new object[] {MonoBehaviourClassId, type.Name, 0});
                }
                return;
            }
        }    }
}
