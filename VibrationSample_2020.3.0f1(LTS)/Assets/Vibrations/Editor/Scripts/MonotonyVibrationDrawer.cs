using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Myspace.Vibrations
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



        private void Init(SerializedProperty property)
        {
            propertyType = typeof(MonotonyVibration);
            mainProperty = (fieldInfo.GetValue(property.serializedObject.targetObject)) as MonotonyVibration;

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

            EditorGUI.BeginProperty(position, label, property);

            HeaderGUI(position, property, label);
            BodyGUI(position, property, label);

            EditorGUI.EndProperty();
        }


        private void HeaderGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }


        private void BodyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            parametorOpen = EditorGUI.Foldout(position,parametorOpen, "MonotonyVibration");

            if (parametorOpen)
            {
                //PropertySerializedGUI();
            }
        }


        #endregion


        #region Sub GUI methods


        private void PropertySerializedGUI()
        {
            EditorGUI.indentLevel++;

            ValueSerializedGUI();

            EditorGUI.indentLevel--;
        }


        private void ValueSerializedGUI()
        {
            var value = (float)fieldValue.GetValue(mainProperty);
            var afterValue = EditorGUILayout.Slider(value, 0, 1);
        }


        #endregion


        #region operator methods



        #endregion
    }
}