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

        foreach (CriticalHitCurvePoint point in curvePoints)
        {
            if (playerLevel >= point.level)
            {
                chance = point.chance;
            }
            else
            {
                break;
            }
        }

        return Mathf.Clamp(chance, 0.0f, 1.0f);
    }
}