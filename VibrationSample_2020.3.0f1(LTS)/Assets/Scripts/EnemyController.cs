using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : CharacterBehabiour
{
    [Header("打つ弾")][SerializeField] BulletController m_bullet = null;
    // 進んでいるのが右かどうか
    private bool m_right;
    // 次の球を打つまでの時間
    private float m_nextShot;
    // 時間計算用
    private float m_time;

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    private void Start()
    {
        m_nextShot = Random.Range(0.01f, 0.5f);
    }

    public override void Move(float sideSize,float speed)
    {
        var position = transform.position;
        float posX = position.x;

        m_time += Time.deltaTime;

        if(m_time > m_nextShot)
        {
            // 時間をリセットし弾を発射
            m_time = 0;
            var bullet = Instantiate(m_bullet, transform.position, transform.rotation);
            GameDirctor.Instans.AddBullet(bullet);

            // 次打つタイミングを決定
            m_nextShot = Random.Range(0.01f, 0.5f);

            // 確率で反転
            if(m_nextShot > 0.3f)
            {
                m_right = !m_right;
            }
        }

        // 移動する距離の算出

        posX += Time.deltaTime * speed * (m_right ? 1:-1);


        // 可動域を超えた場合反転
        if (posX < -sideSize)
        {
            m_right = !m_right;
            posX = -sideSize;
        }
        else if (posX > sideSize)
        {
            m_right = !m_right;
            posX = sideSize;
        }

        // 移動の適用

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
        m_hp--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerAttack")
        {
            GameDirctor.Instans.ReMoveBullet(other.gameObject);
            Damage();
        }
    }
}
