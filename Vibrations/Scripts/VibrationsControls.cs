namespace Myspace.Vibrations
{
    using System.Collections.Generic;
    using XInputDotNetPure;

    #region VibrationsControls is class

    public class VibrationsControls
    {
        private bool m_useVibrations = true;
        private bool m_stop = false;
        private Dictionary<int, VibrationLayer> m_VibrationLayers = new Dictionary<int, VibrationLayer>(4);


        /// <summary>
        /// 現在の振動（毎フレーム更新していないとおかしくなるよ）
        /// </summary>
        public VibrationForce CurentForce { get; private set; } = VibrationForce.Zero;
        /// <summary>
        /// 登録してある振動対応コントローラー
        /// </summary>
        public GamePadState CurrentPad { get; private set; }
        /// <summary>
        /// 登録してある振動対応コントローラーナンバー
        /// </summary>
        private PlayerIndex m_playerIndex;
        ///<summary>
        ///どのくらい精密に振動の値を変動させるか
        ///</summary>
        private float m_vibrationAccuracy = 0.01f;
        /// <summary>
        /// 自動でLayerを削除するかどうか
        /// </summary>
        public bool AutoRemoveLayer { get; set; } = false;


        /// <summary>
        /// どのくらい精密に振動の値を変動させるか（デフォルトにしておきな）
        /// <para>細かい（挙動が荒くなる）　０　～～～　１　荒い（挙動は安定）</para>
        /// <para>変な値にしないように</para>
        /// <para>０～１の間で、勝手に保管するから関係ないけどね。</para>
        /// </summary>
        public float VibrationAccuracy
        {
            get => m_vibrationAccuracy;
            set
            {
                var result = value;

                if (result >= 1) { result = 1; }
                else if (result < 0) { result = 0; }

                m_vibrationAccuracy = result;
            }
        }

        /// <summary>
        /// 振動のON:OFF
        /// 時間の加算は関係なく実行
        /// </summary>
        public bool Active
        {
            get => m_useVibrations;

            set
            {
                m_useVibrations = value;
                if (!value)
                {
                    GamePad.SetVibration(m_playerIndex, 0, 0);
                    CurentForce = VibrationForce.Zero;
                }
            }
        }

        /// <summary>
        /// trueにすると振動が止まるだけでなく
        /// 時間の加算もやめる
        /// </summary>
        public bool Stop
        {
            get => m_stop;
            set
            {
                m_stop = value;
                if (value)
                {
                    GamePad.SetVibration(m_playerIndex, 0, 0);
                    CurentForce = VibrationForce.Zero;
                }
            }
        }

        /// <summary>
        /// レイヤーをすべて削除し振動を止める
        /// </summary>
        public void Reset()
        {
            m_VibrationLayers.Clear();
            GamePad.SetVibration(m_playerIndex, 0, 0);
            CurentForce = VibrationForce.Zero;
        }

        /// <summary>
        /// 振動コントローラー実働
        /// </summary>
        /// <param name="timeScale"></param>
        public void Update(float timeScale)
        {
            if (Stop)
            {
                return;
            }

            int[] keys = new int[m_VibrationLayers.Keys.Count];
            m_VibrationLayers.Keys.CopyTo(keys, 0);

            float resultL = 0;
            float resultR = 0;

            for (int keycnt = 0; keycnt < keys.Length; keycnt++)
            {
                var key = keys[keycnt];
                var vibrationLayer = m_VibrationLayers[key];

                var force = vibrationLayer.UpdateAndResult(timeScale);

                if(resultL < force.Left) { resultL = force.Left; }
                if(resultR < force.Right) { resultR = force.Right; }

                if(AutoRemoveLayer && vibrationLayer.ListCount <= 0)
                {
                    RemoveLayers(key);
                }
            }

            if (Active)
            {
                SetVibratrion(new VibrationForce(resultL,resultR));
            }
        }

        /// <summary>
        /// 自分でレイヤーを作ってから入れる場合（とても非推奨）
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="vibrationLayer"></param>
        public void SetLayer(int layer,VibrationLayer vibrationLayer)
        {
            if(m_VibrationLayers == null)
            {
                return;
            }

            if (m_VibrationLayers.ContainsKey(layer))
            {
                m_VibrationLayers[layer] = vibrationLayer;
            }
            else
            {
                m_VibrationLayers.Add(layer, vibrationLayer);
            }
        }

        /// <summary>
        /// レイヤーが存在するか
        /// </summary>
        /// <param name="layer">レイヤーナンバー</param>
        /// <returns>あるかどうか</returns>
        public bool GetContainsLayer(int layer)
        {
            return m_VibrationLayers.ContainsKey(layer);
        }

        /// <summary>
        /// レイヤーが存在するか
        /// </summary>
        /// <param name="vibrationLayer">レイヤー本体</param>
        /// <returns>あるかどうか</returns>
        public bool GetContainsLayer(VibrationLayer vibrationLayer)
        {
            return m_VibrationLayers.ContainsValue(vibrationLayer);
        }

        /// <summary>
        /// 登録されているLayerナンバーをすべて取得
        /// </summary>
        /// <returns>ナンバー一覧</returns>
        public int[] GetLayersKeys()
        {
            var keys = new int[m_VibrationLayers.Count];
            m_VibrationLayers.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// 該当するレイヤーがあれば返す。なければnullを返す
        /// </summary>
        /// <param name="layer">レイヤーナンバー</param>
        /// <returns>値</returns>
        public VibrationLayer GetVibrationLayer(int layer)
        {
            bool contains = m_VibrationLayers.ContainsKey(layer);
            var vibrationLayer = contains ? m_VibrationLayers[layer] : null;
            return vibrationLayer;
        }

        /// <summary>
        /// 該当するレイヤーを探す。あればoutで取得
        /// </summary>
        /// <param name="layer">レイヤーナンバー</param>
        /// <param name="vibrationLayer">値</param>
        /// <returns>あるかどうか</returns>
        public bool TryGetVibrationLayer(int layer,out VibrationLayer vibrationLayer)
        {
            bool contains = m_VibrationLayers.ContainsKey(layer);
            vibrationLayer = contains ? m_VibrationLayers[layer] : null;
            return contains; ;
        }

        /// <summary>
        /// layerを指定し、値が見つかったら削除
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="vibration"></param>
        public void RemoveVibration(int layer,Vibration vibration)
        {
            if (m_VibrationLayers == null) return;

            if (m_VibrationLayers.ContainsKey(layer))
            {
                m_VibrationLayers[layer].Remove(vibration);

                if (AutoRemoveLayer && m_VibrationLayers[layer].ListCount <= 0)
                {
                    RemoveLayers(layer);
                }
            }
        }

        /// <summary>
        /// Layerを削除
        /// </summary>
        /// <param name="layers"></param>
        public void RemoveLayers(params int[] layers)
        {

            if (m_VibrationLayers == null || layers == null) return;

            int cnt = m_VibrationLayers.Keys.Count;
            for(int i = 0; i < layers.Length; i++)
            {
                if (m_VibrationLayers.ContainsKey(layers[i]))
                {
                    if (--cnt < 1)
                    {
                        Reset();
                        return;
                    }
                    
                    m_VibrationLayers.Remove(layers[i]);
                }
            }
        }

        /// <summary>
        /// レイヤーのストップ状態を変更する
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="stop"></param>
        public void SetLayerStop(int layer, bool stop)
        {
            if (m_VibrationLayers == null) return;

            if (m_VibrationLayers.ContainsKey(layer))
            {
                m_VibrationLayers[layer].Stop = stop;
            }
        }

        /// <summary>
        /// レイヤーのアクティブ状態を変更する
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="active"></param>
        public void SetLayerActive(int layer, bool active)
        {
            if (m_VibrationLayers == null) return;

            if (m_VibrationLayers.ContainsKey(layer))
            {
                m_VibrationLayers[layer].Active = active;
            }

        }

        /// <summary>
        /// レイヤーのストップ状態を取得する
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool GetLayerStop(int layer)
        {
            if (m_VibrationLayers == null) return false;

            if (m_VibrationLayers.ContainsKey(layer))
            {
                return m_VibrationLayers[layer].Stop;
            }
            return false;
        }

        /// <summary>
        /// レイヤーのアクティブ状態を取得する
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool GetLayerActive(int layer)
        {
            if (m_VibrationLayers == null) return false;

            if (m_VibrationLayers.ContainsKey(layer))
            {
                return m_VibrationLayers[layer].Active;
            }
            return false;
        }

        /// <summary>
        /// コントローラーの振動を正確さの値をもとに変更する
        /// <para>第一引数 : 変更予定の値</para>
        /// </summary>
        /// <param name="force">変更予定の値</param>
        public void SetVibratrion(VibrationForce force)
        {
            var zero = (m_vibrationAccuracy == 0);

            var oldforce = CurentForce;

            if (!zero)
            {
                force.Format(m_vibrationAccuracy);
                oldforce.Format(m_vibrationAccuracy);
            }

            if (force != oldforce)
            {
                GamePad.SetVibration(m_playerIndex, force.Left, force.Right);
                CurentForce = force;
            }
        }

        /// <summary>
        /// レイヤーに振動クラスを追加
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="vibration"></param>
        /// <returns>振動クラス（後で割合を変えたい時などは保持）</returns>
        public Vibration AddVibration(int layer, Vibration vibration)
        {
            if (vibration == null) return null;

            if (((m_VibrationLayers.ContainsKey(layer)) == false))
            {
                var vibrationLayer = new VibrationLayer();
                m_VibrationLayers.Add(layer, vibrationLayer);
            }


            m_VibrationLayers[layer].AddVibration(vibration);

            return vibration;
        }

        /// <summary>
        /// レイヤーに振動クラスを追加（割合はマックス）
        /// </summary>
        /// <param name="scriptable"></param>
        /// <returns>振動クラス（後で割合を変えたい時などは保持）</returns>
        public Vibration AddVibration(ScriptableVibration scriptable)
        {
            if (scriptable == null) return null;

            int layer = scriptable.Layer;
            var vibration = new Vibration(scriptable, Percentage.Max);

            if (((m_VibrationLayers.ContainsKey(layer)) == false))
            {
                var vibrationLayer = new VibrationLayer();
                m_VibrationLayers.Add(layer, vibrationLayer);
            }

            m_VibrationLayers[layer].AddVibration(vibration);

            return vibration;
        }

        /// <summary>
        /// レイヤーに振動クラスを追加、割合を自分で指定する
        /// </summary>
        /// <param name="scriptable"></param>
        /// <param name="percentage"></param>
        /// <returns>振動クラス（後で割合を変えたい時などは保持）</returns>
        public Vibration AddVibration(ScriptableVibration scriptable, Percentage percentage)
        {
            if (scriptable == null) return null;

            int layer = scriptable.Layer;
            var vibration = new Vibration(scriptable, percentage);

            if (((m_VibrationLayers.ContainsKey(layer)) == false))
            {
                var vibrationLayer = new VibrationLayer();
                m_VibrationLayers.Add(layer, vibrationLayer);
            }

            m_VibrationLayers[layer].AddVibration(vibration);

            return vibration;
        }

        /// <summary>
        /// 同じ振動コントローラーインデックスであるかどうか
        /// <para>（同じインデックスを使って別々に宣言していたりすると挙動がおかしくなります）</para>
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        public bool EqualsIndex(VibrationsControls controls)
        {
            return this.m_playerIndex == controls.m_playerIndex;
        }

        /// <summary>
        /// 振動レイヤーを共有します
        /// <para>（呼び出した方のレイヤーを使用します）</para>
        /// </summary>
        /// <param name="controls"></param>
        public void CopyThisVibrationMemory(VibrationsControls controls)
        {
            controls.m_VibrationLayers = this.m_VibrationLayers;
        }

        /// <summary>
        /// 振動対応コントローラーを取得
        /// <para>（複数ある場合はindexの小さいのを習得）</para>
        /// </summary>
        /// <param name="vibrationsControls"></param>
        /// <returns>見つかったか否か（見つかった場合はoutから使用）</returns>
        public static bool TryFoundPad(out VibrationsControls vibrationsControls)
        {
            vibrationsControls = null;

            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    vibrationsControls = new VibrationsControls()
                    {
                        CurrentPad = testState,
                        m_playerIndex = testPlayerIndex
                    };
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 振動対応コントローラーを複数取得（４つまで）
        /// </summary>
        /// <param name="vibrationsControls"></param>
        /// <returns>見つかったか否か（見つかった場合はoutから使用）</returns>
        public static bool TryFoundPads(out VibrationsControls[] vibrationsControls)
        {
            vibrationsControls = null;
            var vibrationsControlsList = new List<VibrationsControls>(4);

            bool found = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    var vibrations = new VibrationsControls()
                    {
                        CurrentPad = testState,
                        m_playerIndex = testPlayerIndex
                    };
                    vibrationsControlsList.Add(vibrations);
                    found = true;
                }
            }

            if (found)
            {
                vibrationsControls = vibrationsControlsList.ToArray();
                return true;
            }

            return false;
        }
    }

    #endregion


    #region VibrationLayer is class

    public class VibrationLayer
    {
        // 力の演算元を格納
        private List<Vibration> m_vibrations = new List<Vibration>(8);

        /// <summary>
        /// レイヤーの振動の値適用のON:OFF
        /// <para>（時間の加算は関係なく実行）</para>
        /// </summary>
        public bool Stop { get; set; } = false;
        /// <summary>
        /// trueにすると振動が止まるだけでなく時間の加算もやめる
        /// </summary>
        public bool Active { get; set; } = true;
        /// <summary>
        /// Listに登録されているclassの個数
        /// </summary>
        public int ListCount
        {
            get => m_vibrations.Count;
        }


        /// <summary>
        /// 値の獲得レイヤーから削除できるものを削除
        /// </summary>
        /// <param name="timeScale"></param>
        /// <returns>レイヤーの値習得許可内で値が大きいもの</returns>
        internal VibrationForce UpdateAndResult(float timeScale)
        {
            if (Stop)
            {
                return VibrationForce.Zero;
            }

            float resultL = 0;
            float resultR = 0;

            for (int i = 0, length = m_vibrations.Count; i < length; i++)
            {

                var vibration = m_vibrations[i];

                var finish = vibration.FinishCheck();

                if (finish)
                {
                    m_vibrations.RemoveAt(i);
                    length--;
                    i--;
                    continue;
                }

                if (vibration.Stop)
                {
                    continue;
                }


                if(Active == false || vibration.Active == false)
                {
                    vibration.Time += timeScale;
                    continue;
                }

                var force = vibration.GetForce();

                if (force.Left > resultL) { resultL = force.Left; }
                if (force.Right > resultR) { resultR = force.Right; }

                vibration.Time += timeScale;
            }

            return new VibrationForce(resultL,resultR);
        }

        /// <summary>
        /// レイヤーに振動を追加
        /// </summary>
        /// <param name="vibration"></param>
        /// <returns></returns>
        public Vibration AddVibration(Vibration vibration)
        {
            if (vibration == null) return null;
            m_vibrations.Add(vibration);
            return vibration;
        }

        /// <summary>
        /// レイヤーに振動を追加
        /// </summary>
        /// <param name="scriptable"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public Vibration AddVibration(ScriptableVibration scriptable, Percentage percentage)
        {
            if (scriptable == null) return null;
            var vibration = new Vibration(scriptable, percentage);
            m_vibrations.Add(vibration);
            return vibration;
        }

        /// <summary>
        /// レイヤーに振動を追加
        /// </summary>
        /// <param name="scriptable"></param>
        /// <returns></returns>
        public Vibration AddVibration(ScriptableVibration scriptable)
        {
            if (scriptable == null) return null;
            var vibration = new Vibration(scriptable, Percentage.Max);
            m_vibrations.Add(vibration);
            return vibration;
        }

        /// <summary>
        /// レイヤーの中に値が見つかった場合は削除
        /// </summary>
        /// <param name="vibration"></param>
        public void Remove(Vibration vibration)
        {
            if (vibration == null) return;

            if (m_vibrations.Contains(vibration))
            {
                m_vibrations.Remove(vibration);
            }
        }
    }

    #endregion
}
