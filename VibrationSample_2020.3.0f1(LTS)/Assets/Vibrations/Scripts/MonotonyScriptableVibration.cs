namespace Myspace.Vibrations
{
    using UnityEngine;

    #region // MonotonyScriptableVibration is ScriptableObject implements IVibration

    [CreateAssetMenu(menuName = ("ScriptableObject/Vibration/Monotony"), fileName = ("NewMonotonyVibration"), order = 0)]
    public class MonotonyScriptableVibration : ScriptableVibration
    {
        [Header("êUìÆÇÃã≠Ç≥ÅiÇOÅ`ÇPÅj")] [SerializeField, Range(0, 1)] float m_vibrationForce = 0;
        [Header("êUìÆéûä‘ÅiÇOÇÃèÍçáÇÕñ≥å¿Åj")][SerializeField] float m_vibrationTime = 0;
        private VibrationForce m_force = VibrationForce.Zero;

        private void OnValidate()
        {
            if (m_vibrationForce < 0)
            {
                m_vibrationForce = 0;
            }
            m_force = new VibrationForce(m_vibrationForce);
        }

        protected override void Reset()
        {
            base.Reset();
            m_vibrationForce = 0;
            m_vibrationTime = 0;
        }

        public override bool FinishCheck(float time)
        {
            return (!(m_vibrationTime == 0)) && (time > m_vibrationTime);
        }

        public override VibrationForce GetVibrationForce(float time)
        {
            return m_force;
        }
    }

    #endregion


    #region // MonotonyVibration is class implements IVibration

    public class MonotonyVibration : IVibration
    {
        private float m_vibrationTime = 0;
        private VibrationForce m_force = VibrationForce.Zero;
        
        public MonotonyVibration(VibrationForce force, float time)
        {
            if(force == null)
            {
                m_force = VibrationForce.Zero;
                m_vibrationTime = 0.01f;
            }
            m_force = force;
            m_vibrationTime = time;
        }

        public bool FinishCheck(float time)
        {
            return (!(m_vibrationTime == 0)) && (time > m_vibrationTime);
        }

        public VibrationForce GetVibrationForce(float time)
        {
            return m_force;
        }
    }

    #endregion
}
