using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myspace.Vibrations
{
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
}
