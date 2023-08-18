using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGOriginalDevelopment_MonsterBlueprint : MonoBehaviour
{
    public RPGOriginalDevelopment_CharacterControl targetPlayer;
    public Animator animatorController;
    public float chaseDistance = 30;
    public float attackDistance = 1.5f;
    public float originalMoveSpeed = .8f;
    public float actualSpeedMultiplier;
    public float actualDamageMultiplier;
    public Vector3 lastFramePosition;
    public float maxHealth = 100;
    public float currentHealth = 100;
    public bool isDead;
    public float maxAttackPower;
    public float minAttackPower;

    [System.Serializable]
    public class BodyRegion
    {
        public string name;
        public Collider regionCollider;
        public float maxRegionHealth;
        public float currentRegionHealth;
        public Renderer[] regionAffectedVisuals;
        public Collider[] regionAffectedColliders;
        public float damageMultiplier = 1;
        public bool isRegionDead = false;
        public UnityEngine.Events.UnityEvent onRegionDeathEvent;
        public bool hideColliderAfterDeath = false;

        public float ReceiveDamage(float damageValue)
        {
            currentRegionHealth -= damageValue * damageMultiplier;
            currentRegionHealth = Mathf.Clamp(currentRegionHealth, 0, maxRegionHealth);

            if (currentRegionHealth <= 0)
            {
                if (!isRegionDead)
                {
                    onRegionDeathEvent.Invoke();
                }

                isRegionDead = true;

                if (hideColliderAfterDeath)
                {
                    regionCollider.enabled = false;
                }

                foreach (Renderer visualEffect in regionAffectedVisuals)
                {
                    visualEffect.enabled = false;
                }

                foreach (Collider coll in regionAffectedColliders)
                {
                    coll.enabled = false;
                }
            }

            return damageMultiplier;
        }
    }

    public void AddSpeedEffect(float speedEffect)
    {
        buffsDebuffs.Add(new BuffDebuff("SlowDebuff", -speedEffect));
    }

    public void AddAttackEffect(float attackEffect)
    {
        buffsDebuffs.Add(new BuffDebuff("AttackDebuff", -attackEffect));
    }

    public void AddEffect(BuffDebuff effect)
    {
        buffsDebuffs.Add(new BuffDebuff(effect));
    }

    public BodyRegion[] allBodyRegions;

    public void Attack()
    {
        if (targetPlayer)
        {
            targetPlayer.TakeDamage(Random.Range(minAttackPower, maxAttackPower) * actualDamageMultiplier);
        }
    }

    public void Die()
    {
        animatorController.enabled = false;

        foreach (BodyRegion bodyRegion in allBodyRegions)
        {
            bodyRegion.regionCollider.gameObject.AddComponent<CharacterJoint>();
        }

        foreach (BodyRegion bodyRegion in allBodyRegions)
        {
            Rigidbody[] parentRigidbodies = bodyRegion.regionCollider.GetComponentsInParent<Rigidbody>();

            if (parentRigidbodies.Length >= 2)
            {
                bodyRegion.regionCollider.gameObject.GetComponent<CharacterJoint>().connectedBody = parentRigidbodies[1];
            }
        }
    }

    public void ReceiveDamage(float damageValue, Collider bodyRegionCollider)
    {
        float damageMultiplier = 1;

        foreach (BodyRegion bodyRegion in allBodyRegions)
        {
            if (bodyRegion.regionCollider == bodyRegionCollider)
            {
                damageMultiplier = bodyRegion.ReceiveDamage(damageValue);
            }
        }

        currentHealth -= damageValue * damageMultiplier;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            if (!isDead)
            {
                Die();
            }

            isDead = true;
        }
    }

    public List<BuffDebuff> buffsDebuffs;

    void Update()
    {
        if (isDead)
        {
            return;
        }

        float speedTemp = 0;
        actualSpeedMultiplier = 1;
        actualDamageMultiplier = 1;

        foreach (BuffDebuff buffDebuff in buffsDebuffs)
        {
            actualSpeedMultiplier += buffDebuff.speedEffect;
            actualDamageMultiplier += buffDebuff.damageEffect;
        }

        actualSpeedMultiplier = Mathf.Max(0, actualSpeedMultiplier);

        if (!targetPlayer)
        {
            targetPlayer = FindObjectOfType<RPGOriginalDevelopment_CharacterControl>();
        }
        else
        {
            float distanceTemp = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distanceTemp <= chaseDistance)
            {
                transform.forward = targetPlayer.transform.position - transform.position;
                transform.eulerAngles = Vector3.up * transform.eulerAngles.y;

                if (distanceTemp > attackDistance)
                {
                    transform.Translate(Vector3.forward * Time.deltaTime * actualSpeedMultiplier * originalMoveSpeed);
                    speedTemp = Vector3.Distance(lastFramePosition, transform.position) / Time.deltaTime;
                }
            }
            //TODO: 探索、设计动画控制器 进入攻击距离开始攻击动画
            //animatorController.SetBool("attack", distanceTemp <= attackDistance);
        }
        //TODO: 探索、设计动画控制器
        //animatorController.SetFloat("speed", speedTemp, 0.2f, Time.deltaTime);
        lastFramePosition = transform.position;
    }
}

[System.Serializable]
public class BuffDebuff
{
    public BuffDebuff(BuffDebuff _effect)
    {
        name = _effect.name;
        speedEffect = _effect.speedEffect;
        damageEffect = _effect.damageEffect;
    }

    public BuffDebuff(string _name, float _speedEffect)
    {
        name = _name;
        speedEffect = _speedEffect;
    }

    public BuffDebuff(string _name, float _speedEffect, float _damageEffect)
    {
        name = _name;
        speedEffect = _speedEffect;
        damageEffect = _damageEffect;
    }

    public string name;
    public float speedEffect;
    public float damageEffect;
}
