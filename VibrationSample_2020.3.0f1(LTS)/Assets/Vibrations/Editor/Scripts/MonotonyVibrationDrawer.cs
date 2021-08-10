using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Myspace.Vibrations.Editor
{

    [CustomPropertyDrawer(typeof(MonotonyVibration))]
    public class MonotonyVibrationDrawer : PropertyDrawer
    {
        MonotonyVibration mainProperty;

        bool parametorOpen = true;

        bool init = false;

        Type propertyType;

        SerializedProperty serializedEndTime;
        SerializedProperty serializedValue;

        FieldInfo fieldEndTime;
        FieldInfo fieldValue;

        float propertyHeight;

        private void Init(SerializedProperty property)
        {
            propertyType = typeof(MonotonyVibration);

            var parent = property.GetParent();
            mainProperty = fieldInfo.GetValue(parent) as MonotonyVibration;

            BindingFlags priveteFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;

            serializedEndTime = property.FindPropertyRelative("m_endTime");
            serializedValue = property.FindPropertyRelative("m_value");

            fieldEndTime = propertyType.GetField("m_endTime", priveteFlags);
            fieldValue = propertyType.GetField("m_value", priveteFlags);

            init = true;
        }

        #region main GUI methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (init == false)
            {
                Init(property);
            }

            var beforPos = position;

            EditorGUI.BeginProperty(position, label, property);

            position = HeaderGUI(position, property, label);
            position = BodyGUI(position, property, label);

            EditorGUI.EndProperty();

            propertyHeight = position.y - beforPos.y;
        }


        private Rect HeaderGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            position.height = EditorGUIUtility.singleLineHeight;
            position = position.GetNextLineRect();
            return position;
        }


        private Rect BodyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            parametorOpen = EditorGUI.Foldout(position, parametorOpen, "MonotonyVibration");
            position = position.GetNextLineRect();

            if (parametorOpen)
            {
                position = PropertySerializedGUI(position, property, label);
            }

            EditorGUI.indentLevel--;
            return position;
        }


        #endregion


        #region Sub GUI methods


        private Rect PropertySerializedGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;

            position = ValueSerializedGUI(position, property, label);

            EditorGUI.indentLevel--;

            return position;
        }


        private Rect ValueSerializedGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = (float)fieldValue.GetValue(mainProperty);
            var afterValue = EditorGUI.Slider(position,value, 0, 1);
            position = position.GetNextLineRect();

            return position;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertyHeight;
        }


        #endregion


        #region operator methods



        #endregion
    }
}