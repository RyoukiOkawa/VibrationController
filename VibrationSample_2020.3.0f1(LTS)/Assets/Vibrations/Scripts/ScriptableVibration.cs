using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Myspace.Vibrations
{
    /// <summary>
    /// 振動の値保持用
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/ScriptableVibration", fileName = "NewVibration")]
    public class ScriptableVibration : ScriptableObject
    {
        [SerializeField,Header("振動の情報")] VibrationParameter m_parameter;

        public VibrationParameter Parameter { get => m_parameter; }

        private void OnValidate()
        {
            m_parameter.ValueUpdate();
        }

    }
}
