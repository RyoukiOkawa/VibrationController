namespace Myspace.Vibrations
{
    using UnityEngine;

    #region Vibration is class

    /// <summary>
    /// 振動の個別インスタンス
    /// </summary>
    public class Vibration
    {
        //力の値保持用
        public readonly IVibration m_vibration;


        /// <summary>
        /// 力に対する割合
        /// </summary>
        public Percentage Percentage { get; set; }
        /// <summary>
        /// 振動を始めてからの時間
        /// </summary>
        public float Time { set; get; } = 0;
        /// <summary>
        /// 値を適用するか
        /// </summary>
        public bool Active { set; get; } = true;
        /// <summary>
        /// 終了し、リストから破棄されているか
        /// </summary>
        public bool Finish { private set; get; } = false;
        /// <summary>
        /// 一時停止中であるかどうか
        /// </summary>
        public bool Stop { get; set; } = false;


        /// <summary>
        /// コンストラクタ（割合は設定しないバージョン。(1,1)になります）
        /// </summary>
        /// <param name="vibration"></param>
        public Vibration(IVibration vibration)
        {
            this.m_vibration = vibration;
            Percentage = Percentage.Max;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="vibration"></param>
        /// <param name="percentage"></param>
        public Vibration(IVibration vibration, Percentage percentage)
        {
            this.m_vibration = vibration;
            Percentage = percentage;
        }


        /// <summary>
        /// レイヤーに追加されてから経過した時間における値を取得
        /// </summary>
        /// <returns>力の値</returns>
        public VibrationForce GetForce()
        {
            if (m_vibration == null) return VibrationForce.Zero;
            var force = m_vibration.GetVibrationForce(Time);

            force *= Percentage;
            return force;
        }

        /// <summary>
        /// レイヤーからの削除条件に達しているかチェック
        /// </summary>
        /// <returns>演算の結果</returns>
        public bool FinishCheck() 
        {
            if (m_vibration == null)
            {
                Finish = true;
                return true;
            }

            bool finish = m_vibration.FinishCheck(Time);
            Finish = finish;
            return finish; 
        }
    }

    #endregion 


    #region IVibration is interface こいつを使って自分好みのやつを作ろう！！
    public interface IVibration
    {
        /// <summary>振動が終わっているかどうかチェック</summary>
        /// <param name="time">振動を始めてからの時間</param>
        /// <returns>振動が終わっているかどうか</returns>
        bool FinishCheck(float time);

        /// <summary>力の値を獲得</summary>
        /// <param name="time">振動を始まってからの時間</param>
        /// <returns>力の値</returns>
        VibrationForce GetVibrationForce(float time);
    }
    #endregion


    #region // ScriptableVibration is ScriptableObject as abstract class implements IVibration
    /// <summary>
    /// スクリプタブルオブジェクトのもとになるやつ
    /// </summary>
    public abstract class ScriptableVibration : ScriptableObject, IVibration
    {
        //優先度
        [Header("レイヤーナンバー")][SerializeField] int m_layer;
        public int Layer { get => m_layer; }
        protected virtual void Reset()
        {
            m_layer = 0;
        }
        public abstract VibrationForce GetVibrationForce(float time);
        public abstract bool FinishCheck(float time);
    }
    #endregion


    #region // Percentage is struct

    /// <summary>
    /// 割合（値は０～１）
    /// </summary>
    [System.Serializable]
    public struct Percentage
    {
        [SerializeField,Range(0,1)] float m_right;
        [SerializeField, Range(0, 1)] float m_left;

        public float Left
        {
            set
            {
                var result = value;

                if (result >= 1) { result = 1; }
                else if (result < 0) { result = 0; }

                m_left = result;
            }
            get
            {
                return m_left;
            }
        }
        public float Right
        {
            set
            {
                var result = value;

                if (result >= 1) { result = 1; }
                else if (result < 0) { result = 0; }

                m_right = result;
            }
            get
            {
                return m_right;
            }
        }

        public Percentage(float left, float right)
        {
            this.m_left = 0;
            this.m_right = 0;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 引数一つVer両方同じ
        /// </summary>
        /// <param name="value"></param>
        public Percentage(float value)
        {
            this.m_left = 0;
            this.m_right = 0;
            Left = value;
            Right = value;
        }

        /// <summary>
        /// 引数で割り切り捨てる
        /// </summary>
        /// <param name="value"></param>
        public void Format(float value)
        {
            if (value == 0) return;
            m_right = value * (int)(m_right / value);
            m_left = value * (int)(m_left / value);
        }

        /// <summary>
        /// 割合が最小の状態を返す（０）
        /// </summary>
        public static Percentage Zero { get; } = new Percentage(0,0);

        /// <summary>
        /// 割合が最大の状態を返す（１）
        /// </summary>
        public static Percentage Max { get; } = new Percentage(1,1);

        /// <summary>
        /// 平均値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>平均値</returns>
        public static Percentage GetAverage(params Percentage[] percentages)
        {
            if (percentages == null)
            {
                return Percentage.Zero;
            }

            var Count = percentages.Length;
            float resultL = 0;
            float resultR = 0;
            for (int i = 0; i < Count; i++)
            {
                var percentage = percentages[i];
                resultL += percentage.m_left;
                resultR += percentage.m_right;
            }

            resultL /= Count;
            resultR /= Count;

            return new Percentage(resultL, resultR);
        }

        /// <summary>
        /// 最大値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>最大値</returns>
        public static Percentage GetMax(params Percentage[] percentages)
        {
            if (percentages == null)
            {
                return Percentage.Zero;
            }

            var Count = percentages.Length;
            float resultL = 0;
            float resultR = 0;
            for (int i = 0; i < Count; i++)
            {
                var percentage = percentages[i];
                if (resultL < percentage.m_left)
                {
                    resultL = percentage.m_left;
                }
                if (resultR < percentage.m_right)
                {
                    resultR = percentage.m_right;
                }
            }
            return new Percentage(resultL, resultR);
        }

        /// <summary>
        /// 最小値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>最小値</returns>
        public static Percentage GetMin(params Percentage[] percentages)
        {
            if (percentages == null)
            {
                return Percentage.Zero;
            }

            var Count = percentages.Length;
            float resultL = 1;
            float resultR = 1;
            for (int i = 0; i < Count; i++)
            {
                var percentage = percentages[i];
                if (resultL > percentage.m_left)
                {
                    resultL = percentage.m_left;
                }
                if (resultR > percentage.m_right)
                {
                    resultR = percentage.m_right;
                }
            }
            return new Percentage(resultL, resultR);
        }

        #region // operatorなど

        public static Percentage operator *(Percentage v, float f)
        {
            v.m_left *= f;
            v.m_right *= f;
            return v;
        }
        public static Percentage operator /(Percentage v, float f)
        {
            v.m_left /= f;
            v.m_right /= f;
            return v;
        }
        public static bool operator ==(Percentage a, Percentage b)
        {
            return (a.m_left == b.m_left && a.m_right == b.m_right);
        }
        public static bool operator !=(Percentage a, Percentage b)
        {
            return !(a.m_left == b.m_left && a.m_right == b.m_right);
        }
        public override bool Equals(object other)
        {
            if (other is Percentage v)
            {
                return (this.m_left == v.m_left && this.m_right == v.m_right);
            }
            else if (other is float f)
            {
                return (this.m_left == f && this.m_right == f);
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    #endregion


    #region // VibrationForce is struct

    /// <summary>
    /// 振動の値(０～１)
    /// </summary>
    [System.Serializable]
    public struct VibrationForce
    {
        [SerializeField, Range(0, 1)] float m_right;
        [SerializeField, Range(0, 1)] float m_left;

        public float Left
        {
            set
            {
                var result = value;

                if (result >= 1) { result = 1; }
                else if (result < 0) { result = 0; }

                m_left = result;
            }
            get
            {
                return m_left;
            }
        }
        public float Right
        {
            set
            {
                var result = value;

                if (result >= 1) { result = 1; }
                else if (result < 0) { result = 0; }

                m_right = result;
            }
            get
            {
                return m_right;
            }
        }

        public VibrationForce(float left,float right)
        {
            this.m_left = 0;
            this.m_right = 0;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 引数一つVer両方同じ
        /// </summary>
        /// <param name="value"></param>
        public VibrationForce(float value)
        {
            this.m_left = 0;
            this.m_right = 0;
            Left = value;
            Right = value;
        }

        /// <summary>
        /// 引数で割り切り捨てる
        /// </summary>
        /// <param name="value"></param>
        public void Format(float value)
        {
            if (value == 0) return;
            m_right = value * (int)(m_right / value);
            m_left = value * (int)(m_left / value);
        }

        /// <summary>
        /// 平均値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>平均値</returns>
        public static VibrationForce GetAverage(params VibrationForce[] forces)
        {
            if(forces == null)
            {
                return VibrationForce.Zero;
            }

            var forceCount = forces.Length;
            float resultL = 0;
            float resultR = 0;
            for(int i = 0;i < forceCount; i++)
            {
                var force = forces[i];
                resultL += force.m_left;
                resultR += force.m_right;
            }

            resultL /= forceCount;
            resultR /= forceCount;

            return new VibrationForce(resultL, resultR);
        }

        /// <summary>
        /// 最大値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>最大値</returns>
        public static VibrationForce GetMax(params VibrationForce[] forces)
        {
            if (forces == null)
            {
                return VibrationForce.Zero;
            }

            var forceCount = forces.Length;
            float resultL = 0;
            float resultR = 0;
            for (int i = 0; i < forceCount; i++)
            {
                var force = forces[i];
                if(resultL < force.m_left)
                {
                    resultL = force.m_left;
                }
                if(resultR < force.m_right)
                {
                    resultR = force.m_right;
                }
            }
            return new VibrationForce(resultL, resultR);
        }

        /// <summary>
        /// 最小値を出す
        /// </summary>
        /// <param name="forces"></param>
        /// <returns>最小値</returns>
        public static VibrationForce GetMin(params VibrationForce[] forces)
        {
            if (forces == null)
            {
                return VibrationForce.Zero;
            }

            var forceCount = forces.Length;
            float resultL = 1;
            float resultR = 1;
            for (int i = 0; i < forceCount; i++)
            {
                var force = forces[i];
                if (resultL > force.m_left)
                {
                    resultL = force.m_left;
                }
                if (resultR > force.m_right)
                {
                    resultR = force.m_right;
                }
            }
            return new VibrationForce(resultL, resultR);
        }
        
        /// <summary>
        /// 力が最小の状態を返す（０）
        /// </summary>
        public static VibrationForce Zero { get; } = new VibrationForce(0, 0);

        /// <summary>
        /// 力が最大の状態を返す（１）
        /// </summary>
        public static VibrationForce Max { get; } = new VibrationForce(1, 1);

        #region // operatorなど

        public static VibrationForce operator *(VibrationForce v, Percentage p)
        {
            v.Left *= p.Left;
            v.Right *= p.Right;
            return v;
        }
        public static VibrationForce operator *(VibrationForce v, float f)
        {
            v.m_left *= f;
            v.m_right *= f;
            return v;
        }

        public static VibrationForce operator /(VibrationForce v, float f)
        {
            v.m_left /= f;
            v.m_right /= f;
            return v;
        }
        public static bool operator ==(VibrationForce a, VibrationForce b)
        {
            return (a.m_left == b.m_left && a.m_right == b.m_right);
        }
        public static bool operator !=(VibrationForce a, VibrationForce b)
        {
            return !(a.m_left == b.m_left && a.m_right == b.m_right);
        }
        public static bool operator ==(VibrationForce v, float f)
        {
            return (v.m_left == f && v.m_right == f);
        }
        public static bool operator !=(VibrationForce v, float f)
        {
            return !(v.m_left == f && v.m_right == f);
        }
        public override bool Equals(object other)
        {
            if (other is VibrationForce v)
            {
                return (this.m_left == v.m_left && this.m_right == v.m_right);
            }
            else if (other is float f)
            {
                return (this.m_left == f && this.m_right == f);
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    #endregion

    #endregion
}
