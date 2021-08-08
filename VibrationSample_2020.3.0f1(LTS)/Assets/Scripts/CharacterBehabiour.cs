using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public abstract class CharacterBehabiour : MonoBehaviour
{
    protected int m_hp;
    [SerializeField,Min(1)] protected int m_hpLimmit;

    protected virtual void OnValidate()
    {
        m_hp = m_hpLimmit;
    }

    public abstract void Move(float side, float speed);
    public abstract bool Died();

    public abstract void Damage();
}
