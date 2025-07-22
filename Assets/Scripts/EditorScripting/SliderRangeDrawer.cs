using UnityEditor;
using UnityEngine;

namespace EditorScripting
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    public class SliderRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");
            SerializedProperty rangeMin = property.FindPropertyRelative("rangeMin");
            SerializedProperty rangeMax = property.FindPropertyRelative("rangeMax");

            float fieldWidth = 40.0f;
            float fieldsMargin = 3.0f;

            Rect minFieldRect = new Rect(position.x,
                                         position.y,
                                         fieldWidth,
                                         EditorGUIUtility.singleLineHeight);

            Rect maxFieldRect = new Rect(position.x + position.width - fieldWidth,
                                         position.y,
                                         fieldWidth, 
                                         EditorGUIUtility.singleLineHeight);

            Rect sliderRect = new Rect(position.x + fieldWidth + fieldsMargin, 
                                       position.y,
                                       position.width - 2 * fieldWidth - 2 * fieldsMargin, 
                                       EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(minFieldRect, min, GUIContent.none);
            EditorGUI.PropertyField(maxFieldRect, max, GUIContent.none);

            float minValue = Mathf.RoundToInt(min.floatValue);
            float maxValue = Mathf.RoundToInt(max.floatValue);

            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, rangeMin.floatValue, rangeMax.floatValue);

            min.floatValue = minValue;
            max.floatValue = maxValue;

            EditorGUI.EndProperty();
        }
    }
#endif
}
