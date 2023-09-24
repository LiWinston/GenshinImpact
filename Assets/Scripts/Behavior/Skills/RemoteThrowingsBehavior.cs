using System;
using enemyBehaviour;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class RemoteThrowingsBehavior : MonoBehaviour, IPoolable
{
    public ObjectPool<GameObject> ThisPool { get; set; }
    public bool IsExisting { get; set; }
    internal enum PositionalCategory
    {
        Throwing,
        ImmediatelyInPosition
    };
    [SerializeField] internal PositionalCategory positionalCategory;
    

    [Tooltip("Effect")]
    float damage = 50f;
    float AOEDamage = 10f;
    float AOERange = 10f;
    internal enum EffectCategory
    {
        Explosion,
        Exising,
        Bouncing
    }

    [SerializeField] internal EffectCategory _effectCategory = EffectCategory.Explosion;
    float disappearDistance = 10f;
    float disappearTime = 3f;
    // float RemainingEffectTime = 0f;
    
    public void Release()
    {
        ThisPool.Release(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (positionalCategory == PositionalCategory.Throwing)
        {
            if (other.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                ApplyEffect(other);
            }

            if (other.gameObject.layer == LayerMask.GetMask("Wall"))
            {
                
            }
        }
        else if (positionalCategory == PositionalCategory.ImmediatelyInPosition)
        {
            if (other.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                ApplyEffect(other);
            }
        }
    }
    
    private void ApplyEffect(Collider other)
    {
        var mstbhv = other.GetComponent<MonsterBehaviour>();
        if (mstbhv != null)
        {
            mstbhv.TakeDamage(damage);
        }
        ApplyAOEEffect();
    }
    
    private void ApplyAOEEffect()
    {
        var colliders = Physics.OverlapSphere(transform.position, AOERange, LayerMask.GetMask("Enemy"));
        foreach (var collider in colliders)
        {
            var mstbhv = collider.GetComponent<MonsterBehaviour>();
            if (mstbhv != null)
            {
                mstbhv.TakeDamage(AOEDamage);
            }
        }
    }
}