using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Myspace.Vibrations;

[CustomPropertyDrawer(typeof(VibrationParameter))]
public class VibrationParameterDrawer : PropertyDrawer
{
    VibrationParameter mainProperty;

    bool parametorOpen = true;

    bool init = false;

    Type propertyType;

    FieldInfo twoSideEqual;
    FieldInfo leftValue;
    FieldInfo rightValue;
    FieldInfo endMode;
    FieldInfo endTime;

    SerializedProperty serializedTwoEqual;
    SerializedProperty serializedLeftValue;
    SerializedProperty serializedRightValue;
    SerializedProperty serializedEndMode;
    SerializedProperty serializedEndTime;


    private void Init(SerializedProperty property)
    {
        propertyType = typeof(VibrationParameter);

        mainProperty = (fieldInfo.GetValue(property.serializedObject.targetObject)) as VibrationParameter;

        BindingFlags priveteFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;


        init = true;

        twoSideEqual = propertyType.GetField("m_twoSideEqual", priveteFlags);
        leftValue = propertyType.GetField("m_leftValue", priveteFlags);
        rightValue = propertyType.GetField("m_rightValue", priveteFlags);
        endMode = propertyType.GetField("m_endMode", priveteFlags);
        endTime = propertyType.GetField("m_endTime", priveteFlags);

        serializedTwoEqual = property.FindPropertyRelative("m_twoSideEqual");
        serializedLeftValue = property.FindPropertyRelative("m_leftValue");
        serializedRightValue = property.FindPropertyRelative("m_rightValue");
        serializedEndMode = property.FindPropertyRelative("m_endMode");
        serializedEndTime = property.FindPropertyRelative("m_endTime");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (init == false)
        {
            Init(property);
        }


        EditorGUI.BeginProperty(position, label, property);

        HeaderGUI(position,property,label);
        BodyGUI(position, property, label);
        
        EditorGUI.EndProperty();
    }

    private void HeaderGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    }

    private void BodyGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var beforeIndent = EditorGUI.indentLevel;

        parametorOpen = EditorGUILayout.Foldout(parametorOpen, "VibrationParameter");

        if (parametorOpen)
        {

            EditorGUI.indentLevel++;

            var beforTwoSideEqual = (bool)twoSideEqual.GetValue(mainProperty);

            EditorGUILayout.PropertyField(serializedTwoEqual);

            

            if ((bool)twoSideEqual.GetValue(mainProperty))
            {
                EditorGUILayout.PropertyField(serializedLeftValue, new GUIContent("両辺の値"), null);

                if (GUILayout.Button("値を別々する"))
                {
                    SeparateValues();
                }

                position.y += EditorGUI.GetPropertyHeight(serializedLeftValue, includeChildren : true);

            }
            else
            {
                if (beforTwoSideEqual)
                {
                    SeparateValues();
                }

                EditorGUILayout.PropertyField(serializedLeftValue, new GUIContent("左辺の値"), null);
                EditorGUILayout.PropertyField(serializedRightValue, new GUIContent("右辺の値"), null);

                EditorGUILayout.BeginHorizontal();



                if (GUILayout.Button("値を同じにする(左優先)"))
                {
                    twoSideEqual.SetValue(mainProperty, true);

                    rightValue.SetValue(mainProperty, null);
                }

                if (GUILayout.Button("値を同じにする(右優先)"))
                {
                    twoSideEqual.SetValue(mainProperty, true);

                    leftValue.SetValue(
                        mainProperty,
                        rightValue.GetValue(mainProperty)
                        );

                    rightValue.SetValue(mainProperty, null);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.PropertyField(serializedEndTime, new GUIContent("EndTime"), null);
            EditorGUILayout.PropertyField(serializedEndMode, new GUIContent("EndMode"), null);
            EditorGUI.indentLevel = beforeIndent;
        }
    }


    #region operator methods

    private void SeparateValues()
    {
        twoSideEqual.SetValue(mainProperty, false);

        var iType = typeof(IVibrationValue);
        MethodInfo info = iType.GetMethod("CreateOrigin");


        rightValue.SetValue(
            mainProperty,
            info.Invoke(leftValue.GetValue(mainProperty), null)
            );
    }

    #endregion
}
