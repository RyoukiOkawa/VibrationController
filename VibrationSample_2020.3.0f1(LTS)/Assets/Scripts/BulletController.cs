using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("’e‚Ì”ò‚ñ‚Å‚¢‚­•ûŒü")] [SerializeField] Vector3 m_shot;

    private float m_time = 0;
    public void Move(float speed)
    {
        transform.Translate(m_shot * speed);
    }
    public bool Finish()
    {
        return (m_time > 5);
    }
}
