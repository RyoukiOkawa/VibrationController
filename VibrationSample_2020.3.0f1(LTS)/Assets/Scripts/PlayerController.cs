using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Myspace.Vibrations;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : CharacterBehabiour
{
    private InputAction m_move;
    private InputAction m_shot;

    [SerializeField] HertLayer[] m_hertLayers = null;
    [SerializeField] BulletController m_bullet = null;
    [SerializeField] ScriptableVibration m_damageVibration = null;


    /// <summary>
    /// 配列の要素数が大きいほど振動が変わるタイミングを大きくする
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        int hertChange = -1;
        if (m_hertLayers != null)
        {
            for(int i = 0;i < m_hertLayers.Length;i++)
            {
                HertLayer layer = m_hertLayers[i];

                if(layer.HertChange <= hertChange)
                {
                    layer.HertChange = ++hertChange; 
                }
                else
                {
                    hertChange = layer.HertChange;
                }

                // 振動のインスタンスを作り直す
                layer.VibrationInstans = new Vibration(layer.Vibration);
            }
        }
    }

    private void Start()
    {
        var inputMap = GetComponent<PlayerInput>().currentActionMap;
        m_move = inputMap["Move"];
        m_shot = inputMap["Shot"];

        // 登録されている振動をコントローラーに使用可能にする
        if(m_hertLayers != null)
        {
            for (int i = 0; i < m_hertLayers.Length; i++) 
            {
                 GameDirctor.Instans.Controls.AddVibration(GameDirctor.HertLayer, m_hertLayers[i].VibrationInstans);
            }
        }


        HertChange();
    }
    
    /// <summary>
    /// Hpを見て心臓の動きを変更する
    /// </summary>
    private void HertChange()
    {
        if(m_hertLayers != null)
        {
            bool find = false;

            for(int i = 0;i < m_hertLayers.Length; i++)
            {
                var layer = m_hertLayers[i];
                if(!find && layer.HertChange >= m_hp)
                {
                    find = true;
                    // 登録されている振動を最初から再生させ、Stopをやめる

                    layer.VibrationInstans.Time = 0;
                    layer.VibrationInstans.Stop = false;
                }
                else
                {
                    // 使わない心臓の振動を止める

                    layer.VibrationInstans.Stop = true;
                }
            }
        }
    }

    public override void Move(float sideSize,float speed)
    {

        // 弾を発射

        if (m_shot.triggered)
        {
            var bullet = Instantiate(m_bullet, transform.position, transform.rotation);
            GameDirctor.Instans.AddBullet(bullet);
        }


        // プレイヤーの移動

        var position = transform.position;
        float posX = position.x;
        float inputX = m_move.ReadValue<float>();
        posX += inputX * Time.deltaTime * speed;

        if (posX < -sideSize)
        {
            posX = -sideSize;
        }
        else if(posX > sideSize)
        {
            posX = sideSize;
        }
        position.x = posX;

        transform.position = position;

    }
    public override bool Died()
    {
        bool died = (m_hp <= 0);
        return died;
    }
    public override void Damage()
    {
        GameDirctor.Instans.Controls.AddVibration(0,m_damageVibration);
        m_hp--;
        HertChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyAttack")
        {
            GameDirctor.Instans.ReMoveBullet(other.gameObject);
            Damage();
        }
    }
}
[System.Serializable]
public class HertLayer
{
    [Header("体力がこれ以下になったら切り替え")] [SerializeField] private int m_hertChange = 0;
    [SerializeField] private ScriptableVibration m_vibration = null;
    private Vibration m_vibrationInstans = null;

    public int HertChange { get => m_hertChange;internal set => m_hertChange = value; }
    public ScriptableVibration Vibration { get => m_vibration;internal set => m_vibration = value; }
    public Vibration VibrationInstans { get => m_vibrationInstans;internal set => m_vibrationInstans = value; }
}