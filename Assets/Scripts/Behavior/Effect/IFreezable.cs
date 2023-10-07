using System.Collections;
using UnityEngine;

namespace Behavior.Effect
{
    public interface IFreezable
    {
        Rigidbody Rb { get;}
        bool IsFrozen { get; set; }
        float OriginalMaxMstSpeed { get; set; }
        float MaxSpeed { get; set; }
        float OriginalMoveForce { get; set; }
        float OriginalAttackCooldownInterval{ get; set; }
        void ActivateFreezeMode(float duration, float continuousDamageAmount = 0f, float instantVelocityMultiplier = 0f, float attackCooldownIntervalMultiplier = 3f, float MaxSpeedMultiplier = 0.1f);
        void DeactivateFreezeMode();
        IEnumerator FreezeEffectCoroutine(float duration, float instantVelocityMultiplier = 0.1f, float attackCooldownIntervalMultiplier = 2f, float MaxSpeedMultiplier = 0.36f);
        Coroutine freezeEffectCoroutine { get; set; }
    }
}