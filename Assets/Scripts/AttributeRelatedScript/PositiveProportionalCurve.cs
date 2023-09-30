using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AttributeRelatedScript
{
    [Serializable]
    public class CriticalHitCurvePoint
    {
        [FormerlySerializedAs("level")] public int _x;
        [FormerlySerializedAs("chance")] public float _f_x_;
    }

    public class PositiveProportionalCurve : MonoBehaviour
    {
        [Tooltip("CurveName")]public string CurveName;
        [SerializeField] private bool limitByMINMAX = false;
        public float min = 0f;
        public float max = 1f;
        public List<CriticalHitCurvePoint> curvePoints = new List<CriticalHitCurvePoint>();

        public float CalculateValueAt(int playerLevel)
        {
            if (curvePoints.Count == 0)
            {
                Debug.LogWarning("Critical Hit Curve is empty. Returning default chance.");
                return 0.1f;
            }

            float fx = 0.0f;

            for (int i = 0; i < curvePoints.Count; i++)
            {
                CriticalHitCurvePoint currentPoint = curvePoints[i];

                if (playerLevel >= currentPoint._x)
                {
                    // 如果玩家等级大于或等于当前点的等级
                    if (i < curvePoints.Count - 1)
                    {
                        CriticalHitCurvePoint nextPoint = curvePoints[i + 1];
                        float t = Mathf.InverseLerp(currentPoint._x, nextPoint._x, playerLevel);
                        fx = Mathf.Lerp(currentPoint._f_x_, nextPoint._f_x_, t);
                    }
                    else
                    {
                        fx = currentPoint._f_x_; // 如果玩家等级超过了最高定义的等级点，使用最后一个点的值
                    }
                }
            }

            return limitByMINMAX ? Mathf.Clamp(fx, min, max) : fx;
        }
    }
}