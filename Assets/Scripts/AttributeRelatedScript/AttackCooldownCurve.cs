using System;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeRelatedScript
{
    [Serializable]
    public class AttackCooldownCurvePoint
    {
        public int level;
        public float cooldown;
    }
    /// <summary>
    /// Want to update cd preperly? 此处必须从一级开始填。其余无所谓。一级要作为分子求动画加速
    /// </summary>
    public class AttackCooldownCurve : MonoBehaviour
    {
        public List<AttackCooldownCurvePoint> curvePoints = new List<AttackCooldownCurvePoint>();

        public float CalculateAttackCooldown(int playerLevel)
        {
            if (curvePoints.Count == 0)
            {
                // Debug.LogWarning("Attack Cooldown Curve is empty. Returning default cooldown.");
                return 1.0f;
            }

            float cooldown = curvePoints[0].cooldown;

            for (int i = 0; i < curvePoints.Count; i++)
            {
                AttackCooldownCurvePoint currentPoint = curvePoints[i];

                if (playerLevel >= currentPoint.level)
                {
                    // 如果玩家等级大于或等于当前点的等级
                    if (i < curvePoints.Count - 1)
                    {
                        AttackCooldownCurvePoint nextPoint = curvePoints[i + 1];
                        float t = Mathf.InverseLerp(currentPoint.level, nextPoint.level, playerLevel);
                        cooldown = Mathf.Lerp(currentPoint.cooldown, nextPoint.cooldown, t);
                    }
                    else
                    {
                        cooldown = currentPoint.cooldown; // 如果玩家等级超过了最高定义的等级点，使用最后一个点的值
                    }
                }
            }

            return Mathf.Clamp(cooldown, 0.4f, 1.2f); 
        }
    }
}