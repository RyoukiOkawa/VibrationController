using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Myspace.Vibrations;

public class GameDirctor : MonoBehaviour
{

    public static int HertLayer = 0; 
    public static int DamadeLayer = 1; 
    public static int ShotLayer = 2; 
    public static GameDirctor Instans { get; private set; } = null;
    public VibrationsControls Controls { get; private set; } = null;

    public PlayerController PlayerController { get;private set; } = null;
    public EnemyController EnemyController { get;private set; } = null;

    private List<BulletController> m_bullets = new List<BulletController>(16);

    /// <summary>
    /// リストから球を排除
    /// </summary>
    /// <param name="object"></param>
    public void ReMoveBullet(GameObject @object)
    {
        var mach = m_bullets.Find(bullet => bullet.gameObject == @object);
        
        if(mach != null)
        {
            m_bullets.Remove(mach);
            Destroy(mach.gameObject);
        }
    }
    public void AddBullet(BulletController bullet)
    {
        if (bullet == null)
            return;

        m_bullets.Add(bullet);
    }

    [Header("横の移動可動域")][SerializeField][Range(1,10)] float m_sideLimmit = 1;
    [Header("PlayerとEnemyの移動速度")][SerializeField][Range(1,10)] float m_moveSpeed = 1;
    [Header("PlayerとEnemyの弾の速度")] [SerializeField][Range(1,10)] float m_bulettSpeed = 1;

    // Start is called before the first frame update
    private void Awake()
    {
        // インスタンスの保存
        Instans = this;

        // 振動コントローラの取得
        if (VibrationsControls.TryFoundPad(out VibrationsControls controls))
        {
            Controls = controls;
        }
        else
        {
            // 無ければ作成し
            // StopをtrueにするとListにたまり続けるので注意
            Controls = new VibrationsControls();
            Controls.Active = false;
        }
    }
    void Start()
    {
        // PlayerとEnemyのインスタンスを獲得
        PlayerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        EnemyController = FindObjectOfType(typeof(EnemyController)) as EnemyController;
    }

    // Update is called once per frame
    void Update()
    {
        // Hpがなくなったかチェック

        var playerDied = PlayerController.Died();
        var enmyDied = EnemyController.Died();

        if(playerDied || enmyDied)
        {
            // hpがなくなったら透明に

            if (playerDied)
            {
                PlayerController.gameObject.SetActive(false);
            }
            if (enmyDied)
            {
                EnemyController.gameObject.SetActive(false);
            }

            // 振動を止める

            Controls.Active = false;

            return;
        }
        else
        {
            // 敵とPlayerの移動

            PlayerController.Move(m_sideLimmit,m_moveSpeed);
            EnemyController.Move(m_sideLimmit, m_moveSpeed);
        }


        // 弾の移動

        float speed = m_bulettSpeed * Time.deltaTime;
        var cnt = m_bullets.Count;
        
        if (cnt != 0)
        {
            for(int i = 0;i < cnt; i++)
            {
                var bullet = m_bullets[i];
                bullet.Move(speed);
                if (bullet.Finish())
                {
                    m_bullets.RemoveAt(i);
                    Destroy(bullet.gameObject);
                    i--;
                    cnt--;
                }
            }
        }
    }
    private void LateUpdate()
    {
        Controls.Update(Time.deltaTime);
    }
}
