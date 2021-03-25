namespace Myspace.Vibrations
{
    using UnityEngine;

    #region // CurveScriptableVibration is ScriptableObject implements IVibration

    [CreateAssetMenu(menuName = ("ScriptableObject/Vibration/Curve"), fileName = ("NewCurveVibration"), order = 1)]
    public class CurveScriptableVibration : ScriptableVibration
    {
        [Header("値の強さ（０～１）")][SerializeField] AnimationCurve m_curve;
        [Header("繰り返し回数（０の場合は無限）")][SerializeField] int m_roundCount = 0;
        private float rastFrameTime;

        protected override void Reset()
        {
            base.Reset();
            m_roundCount = 0;
            var keyframeStart = new Keyframe { time = 0, value = 0 };
            var keyframeFinal = new Keyframe { time = 1, value = 0 };
            rastFrameTime = keyframeFinal.time;
            m_curve = new AnimationCurve(keyframeStart, keyframeFinal);
            m_curve.postWrapMode = m_curve.preWrapMode = WrapMode.Loop;
        }
        private void OnValidate()
        {
            if (m_curve == null)
            {
                Reset();
            }
            else
            {
                if(m_roundCount < 0)
                {
                    m_roundCount = 0;
                }

                var keys = m_curve.keys;
                var len = keys.Length;
                for (int i = 0; i < len; i++)
                {
                    var value = keys[i].value;
                    if (value < 0) keys[i].value = 0;
                    else if (value > 1) keys[i].value = 1;
                }
                rastFrameTime = keys[len - 1].time;
                var curve = new AnimationCurve(keys);
                curve.postWrapMode = m_curve.postWrapMode;
                curve.preWrapMode = m_curve.preWrapMode;
                m_curve = curve;
            }
        }

        public override VibrationForce GetVibrationForce(float time)
        {
            bool nullCur = m_curve == null; 
            var force = nullCur ? VibrationForce.Zero : new VibrationForce(m_curve.Evaluate(time));
            return force;
        }

        public override bool FinishCheck(float time)
        {
            return (!(m_roundCount == 0)) && (time > m_roundCount * rastFrameTime);
        }
    }

    #endregion


    #region //CurveVibration is class implements IVibration

    /// <summary>
    /// CurveScriptableVibrationのスクリプトから作るバージョン
    /// <para>（コンストラクタから大概のことは設定してください）</para>
    /// </summary>
    public class CurveVibration : IVibration
    {
        [SerializeField] AnimationCurve m_curve;
        private int m_roundCount = 0;
        private float m_rastFrameTime;

        /// <summary>
        /// コンストラクタです。
        /// <para>繰り返し回数の第二引数をマイナスの値にしても動くけど</para>
        /// <para>そんなことをするようならばuintにして使いづらくするぞ!!</para>
        /// </summary>
        /// <param name="curve">値</param>
        /// <param name="roundCount">繰り返し回数</param>
        public CurveVibration(AnimationCurve curve,int roundCount)
        {
            if(curve == null)
            {
                ///まさかnullを入れないとは思うけど
                ///一応入れたときはすぐ排除されるように調整

                m_roundCount = 1;
                var keyframeStart = new Keyframe { time = 0, value = 0 };
                var keyframeFinal = new Keyframe { time = 0.1f, value = 0 };
                m_rastFrameTime = keyframeFinal.time;
                m_curve = new AnimationCurve(keyframeStart, keyframeFinal);
                m_curve.postWrapMode = m_curve.preWrapMode = WrapMode.Loop;
            }
            else
            {
                //０未満１以上のものを

                m_roundCount = roundCount;
                var keys = curve.keys;
                var len = keys.Length;
                for (int i = 0; i < len; i++)
                {
                    var value = keys[i].value;
                    if (value < 0) keys[i].value = 0;
                    else if (value > 1) keys[i].value = 1;
                }
                m_rastFrameTime = keys[len - 1].time;
                var cur = new AnimationCurve(keys);
                cur.postWrapMode = curve.postWrapMode;
                cur.preWrapMode = curve.preWrapMode;
                m_curve = cur;
            }
        }

        public VibrationForce GetVibrationForce(float time)
        {
            bool nullCur = m_curve == null;
            var force = nullCur ? VibrationForce.Zero : new VibrationForce(m_curve.Evaluate(time));
            return force;
        }

        public bool FinishCheck(float time)
        {
            return (!(m_roundCount == 0)) && (time > m_roundCount * m_rastFrameTime);
        }
    }

    #endregion
}
