using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;

namespace Silksprite.QuestReplacer
{
    public static class GizmoIconsDisabler
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            _ = InitializeOnLoadAsync();
        }

        static async Task InitializeOnLoadAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            DisableGizmoIcons(new []{typeof(QuestReplacer)});
        }

        const int MonoBehaviourClassId = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
        static void DisableGizmoIcons(IEnumerable<Type> types)
        {
            // at least I am going to support Unity 2021 later
            var setIconEnabled = Assembly.GetAssembly(typeof(Editor))
                ?.GetType("UnityEditor.AnnotationUtility")
                ?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            if (setIconEnabled == null)
            {
                return;
            }
            foreach (var type in types)
            {
                setIconEnabled.Invoke(null, new object[] {MonoBehaviourClassId, type.Name, 0});
            }
        }
    }
}
