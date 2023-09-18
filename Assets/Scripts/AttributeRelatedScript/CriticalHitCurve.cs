using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CriticalHitCurvePoint
{
    public int level;
    public float chance;
}

public class CriticalHitCurve : MonoBehaviour
{
    public List<CriticalHitCurvePoint> curvePoints = new List<CriticalHitCurvePoint>();

    public float CalculateCriticalHitChance(int playerLevel)
    {
        if (curvePoints.Count == 0)
        {
            Debug.LogWarning("Critical Hit Curve is empty. Returning default chance.");
            return 0.1f;
        }

        float chance = 0.0f;

        for (int i = 0; i < curvePoints.Count; i++)
        {
            CriticalHitCurvePoint currentPoint = curvePoints[i];

            if (playerLevel >= currentPoint.level)
            {
                // 如果玩家等级大于或等于当前点的等级
                if (i < curvePoints.Count - 1)
                {
                    CriticalHitCurvePoint nextPoint = curvePoints[i + 1];
                    float t = Mathf.InverseLerp(currentPoint.level, nextPoint.level, playerLevel);
                    chance = Mathf.Lerp(currentPoint.chance, nextPoint.chance, t);
                }
                else
                {
                    chance = currentPoint.chance; // 如果玩家等级超过了最高定义的等级点，使用最后一个点的值
                }
            }
        }

        return Mathf.Clamp(chance, 0.0f, 1.0f);
    }
}