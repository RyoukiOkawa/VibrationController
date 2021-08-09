using UnityEngine;

namespace Myspace.Vibrations
{
    [System.Serializable]
    public class MonotonyVibration : IVibrationValue
    {
        [SerializeField]
        [Min(0f)] private float? m_endTime = 0;

        [SerializeField]
        [Range(0f, 1f)] float m_value = 0f;

        public float? EndTime
        {
            get => m_endTime;

            set
            {
                if (value < 0)
                {
                    m_endTime = 0;
                }
                else
                {
                    m_endTime = value;
                }
            }
        }

        public float Value
        {
            get => m_value;

            set
            {
                if (value < 0)
                {
                    m_value = 0;
                }
                else if (value > 1)
                {
                    m_value = 1;
                }
                else
                {
                    m_value = value;
                }
            }
        }


        public MonotonyVibration()
        {
            m_value = 0f;
            m_endTime = 0;
        }

        public MonotonyVibration(float value, float time)
        {
            if (value <= 0f)
            {
                m_value = 0;
                m_endTime = 0.01f;
            }
            m_value = value;
            m_endTime = time;
        }

        public bool FinishCheck(float time)
        {
            if (m_endTime is float endTime)
            {
                return (time > endTime);
            }
            return false;
        }

        public float GetVibrationForce(float time)
        {
            return m_value;
        }

        public IVibrationValue CreateOrigin()
        {
            var me = this;
            var result = new MonotonyVibration()
            {
                m_value = me.m_value,
                m_endTime = me.m_endTime
            };

            return result;
        }

        public IVibrationValue DefaultInstance()
        {
            return new MonotonyVibration();
        }
    }
}
