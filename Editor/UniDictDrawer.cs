using System.Collections.Generic;
using com.underdogg.uniext.Runtime.Dictionary;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.underdogg.uniext.Editor.Dictionary
{
    [CustomPropertyDrawer(typeof(UniDict<,>), true)]
    public sealed class UniDictDrawer : PropertyDrawer
    {
        private const float VerticalSpacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                var entries = property.FindPropertyRelative("_entries");
                var y = foldoutRect.yMax + VerticalSpacing;

                EditorGUI.indentLevel++;
                if (entries == null)
                {
                    var warningRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight * 2f);
                    EditorGUI.HelpBox(warningRect, "UniDict drawer could not find serialized field '_entries'.", MessageType.Error);
                }
                else
                {
                    var entriesHeight = EditorGUI.GetPropertyHeight(entries, true);
                    var entriesRect = new Rect(position.x, y, position.width, entriesHeight);
                    EditorGUI.PropertyField(entriesRect, entries, new GUIContent("Entries"), true);
                    y = entriesRect.yMax + VerticalSpacing;

                    var warnings = CollectValidationWarnings(entries);
                    if (warnings.Count > 0)
                    {
                        var warningText = string.Join("\n", warnings);
                        var warningHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(warningText), position.width);
                        var warningRect = new Rect(position.x, y, position.width, warningHeight);
                        EditorGUI.HelpBox(warningRect, warningText, MessageType.Warning);
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
                return totalHeight;

            totalHeight += VerticalSpacing;

            var entries = property.FindPropertyRelative("_entries");
            if (entries == null)
                return totalHeight + EditorGUIUtility.singleLineHeight * 2f;

            totalHeight += EditorGUI.GetPropertyHeight(entries, true);

            var warnings = CollectValidationWarnings(entries);
            if (warnings.Count > 0)
            {
                var warningText = string.Join("\n", warnings);
                var viewWidth = Mathf.Max(100f, EditorGUIUtility.currentViewWidth - 40f);
                totalHeight += VerticalSpacing + EditorStyles.helpBox.CalcHeight(new GUIContent(warningText), viewWidth);
            }

            return totalHeight;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var entries = property.FindPropertyRelative("_entries");

            var foldout = new Foldout {
                text = property.displayName,
                value = property.isExpanded
            };

            foldout.RegisterValueChangedCallback(evt => property.isExpanded = evt.newValue);
            root.Add(foldout);

            if (entries == null)
            {
                foldout.Add(new UnityEngine.UIElements.HelpBox("UniDict drawer could not find serialized field '_entries'.", HelpBoxMessageType.Error));
                return root;
            }

            var entriesField = new PropertyField(entries, "Entries");
            foldout.Add(entriesField);

            var warningBox = new UnityEngine.UIElements.HelpBox(string.Empty, HelpBoxMessageType.Warning) {
                style = { display = DisplayStyle.None }
            };
            foldout.Add(warningBox);

            void RefreshWarnings()
            {
                var warnings = CollectValidationWarnings(entries);
                if (warnings.Count == 0)
                {
                    warningBox.text = string.Empty;
                    warningBox.style.display = DisplayStyle.None;
                    return;
                }

                warningBox.text = string.Join("\n", warnings);
                warningBox.style.display = DisplayStyle.Flex;
            }

            RefreshWarnings();
            root.TrackPropertyValue(entries, _ => RefreshWarnings());

            return root;
        }

        private static List<string> CollectValidationWarnings(SerializedProperty entries)
        {
            var warnings = new List<string>();
            if (entries == null || !entries.isArray)
                return warnings;

            var seen = new Dictionary<string, int>();
            var hasUnsupportedKeys = false;

            for (var i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                var key = entry.FindPropertyRelative("Key");

                if (key == null)
                {
                    warnings.Add($"Entry {i}: missing serialized field 'Key'.");
                    continue;
                }

                if (IsNullKey(key))
                {
                    warnings.Add($"Entry {i}: key is null.");
                    continue;
                }

                var signature = BuildKeySignature(key);
                if (signature == null)
                {
                    hasUnsupportedKeys = true;
                    continue;
                }

                if (seen.TryGetValue(signature, out var firstIndex))
                {
                    warnings.Add($"Duplicate key detected at indices {firstIndex} and {i}.");
                }
                else
                {
                    seen.Add(signature, i);
                }
            }

            if (hasUnsupportedKeys)
                warnings.Add("Some key types are not validated for duplicates in the inspector.");

            return warnings;
        }

        private static bool IsNullKey(SerializedProperty key)
        {
            switch (key.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return key.objectReferenceValue == null;
                case SerializedPropertyType.ManagedReference:
                    return key.managedReferenceValue == null;
                default:
                    return false;
            }
        }

        private static string BuildKeySignature(SerializedProperty key)
        {
            switch (key.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return $"int:{key.longValue}";
                case SerializedPropertyType.Boolean:
                    return $"bool:{key.boolValue}";
                case SerializedPropertyType.Float:
                    return $"float:{key.floatValue}";
                case SerializedPropertyType.String:
                    return $"string:{key.stringValue}";
                case SerializedPropertyType.Color:
                    return $"color:{key.colorValue}";
                case SerializedPropertyType.ObjectReference:
                    return key.objectReferenceValue == null
                        ? "obj:null"
                        : $"obj:{key.objectReferenceInstanceIDValue}";
                case SerializedPropertyType.Enum:
                    return $"enum:{key.enumValueIndex}";
                case SerializedPropertyType.Vector2:
                    return $"vec2:{key.vector2Value}";
                case SerializedPropertyType.Vector3:
                    return $"vec3:{key.vector3Value}";
                case SerializedPropertyType.Vector4:
                    return $"vec4:{key.vector4Value}";
                case SerializedPropertyType.Rect:
                    return $"rect:{key.rectValue}";
                case SerializedPropertyType.Bounds:
                    return $"bounds:{key.boundsValue}";
                case SerializedPropertyType.Quaternion:
                    return $"quat:{key.quaternionValue}";
                case SerializedPropertyType.Vector2Int:
                    return $"vec2int:{key.vector2IntValue}";
                case SerializedPropertyType.Vector3Int:
                    return $"vec3int:{key.vector3IntValue}";
                case SerializedPropertyType.RectInt:
                    return $"rectint:{key.rectIntValue}";
                case SerializedPropertyType.BoundsInt:
                    return $"boundsint:{key.boundsIntValue}";
                case SerializedPropertyType.LayerMask:
                    return $"layermask:{key.intValue}";
                case SerializedPropertyType.Character:
                    return $"char:{key.intValue}";
                default:
                    return null;
            }
        }
    }
}
