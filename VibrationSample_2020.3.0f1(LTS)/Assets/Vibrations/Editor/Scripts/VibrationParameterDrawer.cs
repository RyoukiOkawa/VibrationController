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

    FieldInfo fieldTwoSideEqual;
    FieldInfo fieldLeftValue;
    FieldInfo fieldRightValue;
    FieldInfo fieldEndMode;
    FieldInfo fieldEndTime;

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

        fieldTwoSideEqual = propertyType.GetField("m_twoSideEqual", priveteFlags);
        fieldLeftValue = propertyType.GetField("m_leftValue", priveteFlags);
        fieldRightValue = propertyType.GetField("m_rightValue", priveteFlags);
        fieldEndMode = propertyType.GetField("m_endMode", priveteFlags);
        fieldEndTime = propertyType.GetField("m_endTime", priveteFlags);

        serializedTwoEqual = property.FindPropertyRelative("m_twoSideEqual");
        serializedLeftValue = property.FindPropertyRelative("m_leftValue");
        serializedRightValue = property.FindPropertyRelative("m_rightValue");
        serializedEndMode = property.FindPropertyRelative("m_endMode");
        serializedEndTime = property.FindPropertyRelative("m_endTime");
    }

    #region main GUI methods

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
        parametorOpen = EditorGUILayout.Foldout(parametorOpen, "VibrationParameter");

        if (parametorOpen)
        {
            PropertySerializedGUI();
        }
    }


    #endregion


    #region Sub GUI methods


    private void PropertySerializedGUI()
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(serializedTwoEqual);

        if ((bool)fieldTwoSideEqual.GetValue(mainProperty))
        {
            SingleValueSerializedGUI();
        }
        else
        {
            TwoValueSerializedGUI();
        }

        EndModeSerializedGUI();
        EndTimeSerializedGUI();

        EditorGUI.indentLevel--;
    }


    private void SingleValueSerializedGUI()
    {
        EditorGUILayout.PropertyField(serializedLeftValue, new GUIContent("両辺の値"), options : null);

        EditorGUI.indentLevel++;

        if (GUILayout.Button("値を別々する"))
        {
            SeparateValues();
        }

        EditorGUI.indentLevel--;
    }


    private void TwoValueSerializedGUI()
    {

        EditorGUILayout.PropertyField(serializedLeftValue, new GUIContent("左辺の値"),options : null);
        EditorGUILayout.PropertyField(serializedRightValue, new GUIContent("右辺の値"),options : null);

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("値を同じにする(左優先)"))
        {
            EqualsValuesBaseLeft();
        }

        if (GUILayout.Button("値を同じにする(右優先)"))
        {
            EqualsValuesBaseRight();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void EndModeSerializedGUI()
    {
        EditorGUILayout.PropertyField(serializedEndMode, new GUIContent("EndMode"), options : null);

        var endMode = (VibrationEndMode)fieldEndMode.GetValue(mainProperty);

        var values = Enum.GetValues(typeof(VibrationEndMode));

        EditorGUI.indentLevel++;

        foreach(VibrationEndMode value in values)
        {
            var mach = (endMode == value);

            var check = EditorGUILayout.ToggleLeft(new GUIContent(value.ToString()), mach,options : null);

            if(check && (mach == false))
            {
                fieldEndMode.SetValue(mainProperty, value);
            }
        }

        EditorGUI.indentLevel--;
    }

    private void EndTimeSerializedGUI()
    {
        var serializedName = "EndTime";

        EditorGUILayout.PropertyField(serializedEndTime, new GUIContent("EndTime"), options: null);

        var endMode = (VibrationEndMode)fieldEndMode.GetValue(mainProperty);

        

        if (endMode == VibrationEndMode.Custom)
        {
            EditorGUILayout.PropertyField(serializedEndTime, options: null);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(serializedName);

            if (endMode == VibrationEndMode.Infinite)
            {
                EditorGUILayout.LabelField("Infinite : ∞");
            }

            EditorGUILayout.EndHorizontal();
        }

    }

    #endregion


    #region operator methods

    private void SeparateValues()
    {
        fieldTwoSideEqual.SetValue(mainProperty, false);

        var iType = typeof(IVibrationValue);
        MethodInfo info = iType.GetMethod("CreateOrigin");
        var copyValue = info.Invoke(fieldLeftValue.GetValue(mainProperty),parameters : null);

        fieldRightValue.SetValue(mainProperty,copyValue);
    }

    private void EqualsValuesBaseLeft()
    {
        fieldTwoSideEqual.SetValue(mainProperty, true);

        fieldRightValue.SetValue(mainProperty,value : null);
    }

    private void EqualsValuesBaseRight()
    {
        fieldTwoSideEqual.SetValue(mainProperty, true);

        var rightValue = fieldRightValue.GetValue(mainProperty);

        fieldLeftValue.SetValue(mainProperty,rightValue);
        fieldRightValue.SetValue(mainProperty,value : null);
    }

    #endregion
}
