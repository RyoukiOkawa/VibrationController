using UnityEngine;

namespace Myspace.Vibrations
{

    [System.Serializable]
    public class VibrationParameter
    {
        [SerializeField] float m_endTime = 0;
        [SerializeField] VibrationEndMode m_endMode;
        [SerializeField] bool m_twoSideEqual = true;
        [SerializeReference, SubclassSelector] IVibrationValue m_leftValue;
        [SerializeReference, SubclassSelector] IVibrationValue m_rightValue;


        public bool TwoSideEqual { get => m_twoSideEqual;}

        public IVibrationValue LeftValue { get => m_leftValue;}
        public IVibrationValue RightValue { get => m_rightValue;}

        public VibrationEndMode EndMode => m_endMode;

        public float EndTime
        {
            get => m_endTime;
            set => m_endTime = value < 0 ? 0.01f : value;
        }

        public VibrationParameter(IVibrationValue vibrationValue,VibrationEndMode endMode = VibrationEndMode.Left,float? endTime = null)
        {
            m_leftValue =
                m_rightValue = vibrationValue;

            m_twoSideEqual = true;

            m_endMode = endMode;

            if (endTime is float time)
            {
                m_endTime = time;
            }
        }

        public VibrationParameter(IVibrationValue leftVibrationValue, IVibrationValue rightVibrationValue,
            VibrationEndMode endMode = VibrationEndMode.TwoSide,float? endTime = null)
        {
            if (leftVibrationValue == rightVibrationValue)
            {
                m_leftValue =
                    m_rightValue = leftVibrationValue;

                m_twoSideEqual = true;
                switch (endMode)
                {
                    case VibrationEndMode.TwoSide:
                    case VibrationEndMode.Left:
                    case VibrationEndMode.Right:
                        m_endMode = VibrationEndMode.Single;
                        break;
                    default:
                        m_endMode = endMode;
                        break;
                }
            }
            else
            {
                m_leftValue = leftVibrationValue;
                m_rightValue = rightVibrationValue;

                m_twoSideEqual = false;
                m_endMode = endMode;
            }

            
            if (endTime is float time)
            {
                m_endTime = time;
            }
        }

        public void ValueUpdate()
        {
            if (m_leftValue is IAdjust leftAdjust)
            {
                leftAdjust.ValueUpdate();
            }
            if (m_rightValue is IAdjust RightAdjust)
            {
                RightAdjust.ValueUpdate();
            }
        }

        public VibrationForce GetVibrationForce(float time)
        {
            var result = VibrationForce.Zero;

            if (!m_twoSideEqual)
            {
                result.Left = m_leftValue.GetVibrationForce(time);
                result.Right = m_rightValue.GetVibrationForce(time);
            }
            else
            {
                result = new VibrationForce(m_leftValue.GetVibrationForce(time));
            }

            return result;
        }

        public bool FinishCheck(float time)
        {
            if(m_endMode == VibrationEndMode.Custom)
            {
                return m_endTime <= time;
            }
            
            if(m_endMode == VibrationEndMode.Infinite)
            {
                return false;
            }

            bool result = false;

            bool left = m_leftValue != null ?
                m_leftValue.FinishCheck(time) : true;

            bool right = m_rightValue != null ?
                m_rightValue.FinishCheck(time) : true;

            switch (m_endMode)
            {
                case VibrationEndMode.TwoSide:
                    result = (left && right);
                    break;
                case VibrationEndMode.Left:
                case VibrationEndMode.Single:
                    result = left;
                    break;
                case VibrationEndMode.Right:
                    result = right;
                    break;
            }

            return result;
        }
    }

    public enum VibrationEndMode
    {
        TwoSide,
        Left,
        Right,
        Single,
        Custom,
        Infinite
    }
}