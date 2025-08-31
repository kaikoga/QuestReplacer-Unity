using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    [CustomPropertyDrawer(typeof(QuestReplacement))]
    public class QuestReplacementDrawer : PropertyDrawer
    {
        static bool _showDuplicateButton;
        static string _duplicateButtonClickedPath = "";

        public static void BeginShowDuplicateButton() => _showDuplicateButton = true;
        public static void EndShowDuplicateButton()
        {
            _duplicateButtonClickedPath = "";
            _showDuplicateButton = false;
        }

        static readonly Regex ArrayPattern = new Regex(@"^(?:[a-zA-Z0-9_]*)\.Array\.data\[([0-9]*)\]$");
        public static bool DuplicateButtonClicked(out int index)
        {
            index = -1;
            var match = ArrayPattern.Match(_duplicateButtonClickedPath);
            return match.Success && int.TryParse(match.Groups[1].Value, out index);
        }
        
        static bool LikelyUnset(SerializedProperty serializedLeft, SerializedProperty serializedRight)
        {
            var left = serializedLeft.objectReferenceInstanceIDValue;
            var right = serializedRight.objectReferenceInstanceIDValue;
            return left == right || (left != 0 && right == 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth /= 3;
            var width = position.width / 2f - 6f;
            var left = new Rect(position.x, position.y, width, position.height); 
            var center = new Rect(position.x + width, position.y, 12f, position.height); 
            var right = new Rect(position.xMax - width, position.y, width, position.height);
            var serializedLeft = property.FindPropertyRelative("left");
            var serializedRight = property.FindPropertyRelative("right");
            EditorGUI.PropertyField(left, serializedLeft);
            GUI.Label(center, serializedLeft.objectReferenceInstanceIDValue == serializedRight.objectReferenceInstanceIDValue ? "=" : "/");
            if (_showDuplicateButton && LikelyUnset(serializedLeft, serializedRight))
            {
                right.width -= 24;
                EditorGUI.PropertyField(right, serializedRight);
                right.x = right.xMax + 6f;
                right.width = 18f;
                if (GUI.Button(right, "+"))
                {
                    _duplicateButtonClickedPath = property.propertyPath;
                }
            }
            else
            {
                EditorGUI.PropertyField(right, serializedRight);
            }

            EditorGUIUtility.labelWidth = labelWidth;
        }
    }

    public class ShowDuplicateButtonScope : IDisposable
    {
        public ShowDuplicateButtonScope() => QuestReplacementDrawer.BeginShowDuplicateButton();
        public bool DuplicateButtonClicked(out int index) => QuestReplacementDrawer.DuplicateButtonClicked(out index);
        void IDisposable.Dispose() => QuestReplacementDrawer.EndShowDuplicateButton();
    }
}