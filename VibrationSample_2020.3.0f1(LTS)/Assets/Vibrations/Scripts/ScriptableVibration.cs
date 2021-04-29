using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Myspace.Vibrations
{
    /// <summary>
    /// 振動の値保持用
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/ScriptableVibration",fileName = "NewVibration")]
    public class ScriptableVibration : ScriptableObject
    {
        [SerializeField]VibrationParameter m_parameter;

        public VibrationParameter Parameter { get => m_parameter; internal set => m_parameter = value; }

        private void OnValidate()
        {
            m_parameter.ValueUpdate();
        }

    }


#if UNITY_EDITOR

    #region Expansions ScriptableVibration Inspector


    [CustomEditor(typeof(ScriptableVibration))]
    class ScriptableVibrationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tar = target as ScriptableVibration;
            var parameter = tar.Parameter;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("値を同じにする(左優先)"))
            {
                parameter.TwoSideEqual = true;

                parameter.RightValue = parameter.LeftValue;
            }

            if (GUILayout.Button("値を同じにする(右優先)"))
            {
                parameter.TwoSideEqual = true;

                parameter.LeftValue = parameter.RightValue;
            }

            EditorGUILayout.EndHorizontal();

            if (parameter.TwoSideEqual)
            {
                parameter.RightValue = parameter.LeftValue;
            }
            else
            {
                if(parameter.LeftValue == parameter.RightValue)
                {
                    if(parameter.RightValue !=null)
                        parameter.RightValue = parameter.RightValue.CreateOrigin();
                }
            }

        }
    }

    #endregion

#endif
}
