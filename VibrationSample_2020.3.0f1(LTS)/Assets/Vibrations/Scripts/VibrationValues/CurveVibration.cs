using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myspace.Vibrations
{
    public class CurveVibration : IVibrationValue, IAdjust
    {
        [SerializeField, Header("値算出用カーブ")]
        AnimationCurve m_curve;
        [SerializeField, Header("繰り返し回数（０は無限）")]
        [Min(0)] int m_roundCount = 0;
        public AnimationCurve Curve { get => m_curve; set => m_curve = value; }
        public float RastFrameTime { get; private set; }
        public int RoundCount { get => m_roundCount; set => m_roundCount = value; }

        public float? EndTime
        {
            get
            {
                if (m_curve == null)
                    return null;

                var keys = m_curve.keys;
                var lastKey = keys[keys.Length - 1];

                return lastKey.time;
            }
        }

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

        public IVibrationValue DefaultInstance()
        {
            return new CurveVibration();
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

}