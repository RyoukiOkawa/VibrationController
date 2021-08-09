using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Myspace.Vibrations;

[CustomPropertyDrawer(typeof(VibrationParameter))]
public class VibrationParameterDrawer : PropertyDrawer
{
    float propertyHeight;

    VibrationParameter mainProperty;

    bool parametorOpen = true;

    bool init = false;

    Type propertyType;

    Type[] subPropertyTypes; 

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

        var vibrationBaseType = typeof(IVibrationValue);

        subPropertyTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => vibrationBaseType.IsAssignableFrom(t) && t.IsClass && t != vibrationBaseType)
            .ToArray();

        BindingFlags priveteFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;

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

        position = HeaderGUI(position,property,label);
        position = BodyGUI(position, property, label);

        EditorGUI.EndProperty();

        propertyHeight = position.y - beforPos.y;
    }


    private Rect HeaderGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        position.height = EditorGUIUtility.singleLineHeight;
        position = GetNextLineRect(position);
        return position;
    }


    private Rect BodyGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.indentLevel++;
        parametorOpen = EditorGUI.Foldout(position,parametorOpen, "VibrationParameter");
        position = GetNextLineRect(position);

        if (parametorOpen)
        {
            position = PropertySerializedGUI(position,property,label);
        }

        EditorGUI.indentLevel--;
        return position;
    }


    #endregion


    #region Sub GUI methods


    private Rect PropertySerializedGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.indentLevel++;

        var height = EditorGUI.GetPropertyHeight(serializedTwoEqual);
        position.height = height;
        EditorGUI.PropertyField(position,serializedTwoEqual);
        position = GetNextLineRect(position);

        if ((bool)fieldTwoSideEqual.GetValue(mainProperty))
        {
            position = SingleValueSerializedGUI(position, property, label);
        }
        else
        {
            position = TwoValueSerializedGUI(position, property, label);
        }

        position = EndModeSerializedGUI(position, property, label);
        position = EndTimeSerializedGUI(position, property, label);

        EditorGUI.indentLevel--;

        return position;
    }


    private Rect SingleValueSerializedGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var height = EditorGUI.GetPropertyHeight(serializedLeftValue);
        position.height = height;
        EditorGUI.PropertyField(position,serializedLeftValue, new GUIContent("両辺の値"));
        position = GetNextLineRect(position);

        EditorGUI.indentLevel++;


        if (GUI.Button(position,"値を別々する"))
        {
            SeparateValues();
        }
        position = GetNextLineRect(position);

        EditorGUI.indentLevel--;

        return position;
    }


    private Rect TwoValueSerializedGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var leftHeight = EditorGUI.GetPropertyHeight(serializedLeftValue);
        var rightHeight = EditorGUI.GetPropertyHeight(serializedRightValue);
        var width = position.width;

        position.height = leftHeight;
        EditorGUI.PropertyField(position, serializedLeftValue, new GUIContent("左辺の値"));
        position = GetNextLineRect(position);
        position.height = rightHeight;
        EditorGUI.PropertyField(position, serializedRightValue, new GUIContent("右辺の値"));
        position = GetNextLineRect(position);


        EditorGUI.indentLevel++;

        var befourButtonPos = position;

        width = position.width;

        position.width = width / 2;

        if (GUI.Button(position,"値を同じにする(左優先)"))
        {
            EqualsValuesBaseLeft();
        }

        position.x += width / 2;

        if (GUI.Button(position,"値を同じにする(右優先)"))
        {
            EqualsValuesBaseRight();
        }

        var afterButtonPos = position;

        position = new Rect(
            x: befourButtonPos.x,
            y: befourButtonPos.y + afterButtonPos.height,
            width: befourButtonPos.width,
            height: EditorGUIUtility.singleLineHeight
            );

        EditorGUI.indentLevel--;

        return position;
    }

    private Rect EndModeSerializedGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var singleLineHeight = EditorGUIUtility.singleLineHeight;

        position.height = EditorGUI.GetPropertyHeight(serializedEndMode);
        EditorGUI.PropertyField(position,serializedEndMode, new GUIContent("EndMode"));
        position = GetNextLineRect(position);


        var endMode = (VibrationEndMode)fieldEndMode.GetValue(mainProperty);

        var values = Enum.GetValues(typeof(VibrationEndMode));

        EditorGUI.indentLevel++;

        foreach (VibrationEndMode value in values)
        {
            var mach = (endMode == value);

            var check = EditorGUI.ToggleLeft(position,new GUIContent(value.ToString()),mach);
            position = GetNextLineRect(position);

            if (check && (mach == false))
            {
                fieldEndMode.SetValue(mainProperty, value);
            }
        }

        EditorGUI.indentLevel--;

        return position;
    }

    private Rect EndTimeSerializedGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var serializedName = "EndTime";

        position.height = EditorGUI.GetPropertyHeight(serializedEndTime);
        EditorGUI.PropertyField(position,serializedEndTime, new GUIContent("EndTime"));
        position = GetNextLineRect(position);

        var endMode = (VibrationEndMode)fieldEndMode.GetValue(mainProperty);



        if (endMode == VibrationEndMode.Custom)
        {
            EditorGUI.PropertyField(position,serializedEndTime);
            position = GetNextLineRect(position);
        }
        else
        {
            EditorGUI.LabelField(position,serializedName);
            position = GetNextLineRect(position);

            if (endMode == VibrationEndMode.Infinite)
            {
                EditorGUI.LabelField(position,"Infinite : ∞");
                position = GetNextLineRect(position);
            }

        }

        return position;
    }

    public static Rect GetNextLineRect(Rect rect,float ySpace = 0,float? height = null)
    {
        var result = rect;

        result.y += rect.height + ySpace;

        result.height =
            (height is float hei)
            ? hei
            : EditorGUIUtility.singleLineHeight;

        return result;
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


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }
}
