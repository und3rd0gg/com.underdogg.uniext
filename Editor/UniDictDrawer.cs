using com.underdogg.uniext.Runtime.Dictionary;
using UnityEditor;
using UnityEngine;

namespace com.underdogg.uniext.Editor.Dictionary
{
    [CustomPropertyDrawer(typeof(UniDict<,>))]
    public class UniDictDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

            if (property.isExpanded)
            {
                var entries = property.FindPropertyRelative("_entries");
                var currentPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

                for (var i = 0; i < entries.arraySize; i++)
                {
                    var entry = entries.GetArrayElementAtIndex(i);
                    var key = entry.FindPropertyRelative("Key");
                    var value = entry.FindPropertyRelative("Value");

                    var keyWidth = position.width * 0.4f;
                    var valueWidth = position.width * 0.6f - ButtonWidth - 5;

                    var keyRect = new Rect(currentPos.x, currentPos.y, keyWidth, EditorGUIUtility.singleLineHeight);
                    var valueRect = new Rect(keyRect.xMax + 5, currentPos.y, valueWidth, EditorGUIUtility.singleLineHeight);
                    var removeButtonRect = new Rect(valueRect.xMax + 5, currentPos.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

                    EditorGUI.PropertyField(keyRect, key, GUIContent.none);
                    EditorGUI.PropertyField(valueRect, value, GUIContent.none);

                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        entries.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    currentPos.y += EditorGUIUtility.singleLineHeight;
                }

                var addButtonRect = new Rect(position.x + (position.width - ButtonWidth) / 2, currentPos.y, ButtonWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(addButtonRect, "+"))
                {
                    entries.InsertArrayElementAtIndex(entries.arraySize);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var entries = property.FindPropertyRelative("_entries");
                totalHeight += (entries.arraySize + 1) * EditorGUIUtility.singleLineHeight;
            }

            return totalHeight;
        }
    }
}
