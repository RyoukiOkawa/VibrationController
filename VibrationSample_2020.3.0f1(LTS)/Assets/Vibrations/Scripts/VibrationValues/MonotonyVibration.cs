using UnityEngine;

namespace Myspace.Vibrations
{
    public class MonotonyVibration : IVibrationValue
    {
        [SerializeField, Header("U“®‚·‚éŽžŠÔ")]
        [Min(0f)] private float m_vibrationTime = 0;

        [SerializeField, Header("U“®‚Ì’l")]
        [Range(0f, 1f)] float m_value = 0f;

        public float VibrationTime
        {
            get => m_vibrationTime;

            set
            {
                if (value < 0)
                {
                    m_vibrationTime = 0;
                }
                else
                {
                    m_vibrationTime = value;
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
            m_vibrationTime = 0;
        }

        public MonotonyVibration(float value, float time)
        {
            if (value <= 0f)
            {
                m_value = 0;
                m_vibrationTime = 0.01f;
            }
            m_value = value;
            m_vibrationTime = time;
        }

        public bool FinishCheck(float time)
        {
            return (time > m_vibrationTime);
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
                m_vibrationTime = me.m_vibrationTime
            };

            return result;
        }
    }
}
