using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public static class QuestReplacerGUI
    {
        internal static readonly GUIStyle HeaderStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(-4, 0, 4, 0)
        };

        public static void Header(Rect position, string content) => EditorGUI.LabelField(position, content, HeaderStyle);
    }

    public static class QuestReplacerGUILayout
    {
        public static void Header(string content) => EditorGUILayout.LabelField(content, QuestReplacerGUI.HeaderStyle);
    }
}
