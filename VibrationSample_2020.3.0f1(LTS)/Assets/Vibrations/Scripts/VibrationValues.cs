namespace Myspace.Vibrations
{
    using UnityEngine;



    #region interface

    /// <summary>
    /// 振動の値を算出する為のインターフェース
    /// </summary>
    public interface IVibrationValue
    {
        /// <summary>振動が終わっているかどうかチェック</summary>
        /// <param name="time">振動を始めてからの時間</param>
        /// <returns>振動が終わっているかどうか</returns>
        bool FinishCheck(float time);

        /// <summary>力の値を獲得</summary>
        /// <param name="time">振動を始まってからの時間</param>
        /// <returns>力の値</returns>
        float GetVibrationForce(float time);

        /// <summary>
        /// 同じ値の違うインスタンスを作り出す
        /// </summary>
        /// <returns>インスタンス</returns>
        IVibrationValue CreateOrigin();
    }

    public interface IAdjust
    {
        void ValueUpdate();
    }

    #endregion


    #region // MonotonyVibration is class implements IVibrationValue

    public class MonotonyVibration : IVibrationValue
    {
        [SerializeField, Header("振動する時間")]
        [Min(0f)] private float m_vibrationTime = 0;

        [SerializeField, Header("振動の値")]
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

        public override bool Equals(object obj)
        {
            if (obj is MonotonyVibration monotony)
            {
                return (m_value == monotony.m_value && m_vibrationTime == monotony.m_vibrationTime);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
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

    #endregion


    #region //CurveVibration is class implements IVibrationValue,IAdjust

    public class CurveVibration : IVibrationValue, IAdjust
    {
        [SerializeField, Header("値算出用カーブ")]
        AnimationCurve m_curve;
        [SerializeField, Header("繰り返し回数（０は無限）")]
        [Min(0)] int m_roundCount = 0;
        public AnimationCurve Curve { get => m_curve; set => m_curve = value; }
        public float RastFrameTime { get; private set; }
        public int RoundCount { get => m_roundCount; set => m_roundCount = value; }


        public CurveVibration()
        {
            Reset();
        }

        public CurveVibration(AnimationCurve curve, int roundCount)
        {
            if (curve == null)
            {
                /// まさかnullを入れないとは思うけど
                /// 一応入れたときはすぐ排除されるように調整

                m_roundCount = 1;
                RastFrameTime = 0.1f;

                m_curve = AnimationCurve.Linear(0, 0, 0.1f, 0);
                m_curve.postWrapMode = m_curve.preWrapMode = WrapMode.Loop;

            }
            else
            {
                m_roundCount = roundCount;
                m_curve = curve;

                ValueUpdate();
            }
        }

        public CurveVibration(CurveVibration curveVibration)
        {
            m_curve = new AnimationCurve(curveVibration.m_curve.keys);
            m_roundCount = curveVibration.m_roundCount;
            RastFrameTime = curveVibration.RastFrameTime;
        }

        public void Reset()
        {
            m_roundCount = 0;
            RastFrameTime = 0;

            m_curve = AnimationCurve.Linear(0, 0, 1, 0);
            m_curve.postWrapMode = m_curve.preWrapMode = WrapMode.Loop;
        }


        public void ValueUpdate()
        {
            var keys = m_curve.keys;
            var len = keys.Length;
            for (int i = 0; i < len; i++)
            {
                var value = keys[i].value;
                if (value < 0)
                {
                    keys[i].value = 0;
                }
                else if (value > 1)
                {
                    keys[i].value = 1;
                }
            }
            RastFrameTime = keys[len - 1].time;
            var curve = new AnimationCurve(keys);
            curve.postWrapMode = m_curve.postWrapMode;
            curve.preWrapMode = m_curve.preWrapMode;
            m_curve = curve;

        }

        public float GetVibrationForce(float time)
        {
            bool finish = FinishCheck(time);
            var force = finish ? 0 : m_curve.Evaluate(time);
            return force;
        }

        public bool FinishCheck(float time)
        {
            return (!(m_roundCount == 0)) && (time > m_roundCount * RastFrameTime);
        }
        public IVibrationValue CreateOrigin()
        {
            var me = this;

            var result = new CurveVibration()
            {
                m_curve = new AnimationCurve(me.m_curve.keys),
                m_roundCount = me.m_roundCount,
                RastFrameTime = me.RastFrameTime
            };

            return result;
        }
    }

    #endregion


}
