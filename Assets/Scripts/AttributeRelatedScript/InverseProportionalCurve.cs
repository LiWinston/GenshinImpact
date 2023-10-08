using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AttributeRelatedScript
{
    [Serializable]
    public class AttackCooldownCurvePoint
    {
        public int _x;
        [FormerlySerializedAs("cooldown")] public float _f_x_;
    }
    /// <summary>
    /// 伤害冷却时间曲线 或用于类似的反比例曲线 Damage cooldown curve or used for a similar inverse scale curve
    /// </summary>
    public class InverseProportionalCurve : MonoBehaviour
    {
        [Tooltip("CurveName")]public string CurveName;
        [SerializeField] private bool limitByMINMAX = false;
        public float min = 0.4f;
        public float max = 1.2f;
        public List<AttackCooldownCurvePoint> curvePoints = new List<AttackCooldownCurvePoint>();
        

        public float CalculateValueAt(float playerLevel)
        {
            if (curvePoints.Count == 0)
            {
                // Debug.LogWarning("Attack Cooldown Curve is empty. Returning default cooldown.");
                return 1.0f;
            }

            int startIndex = 0;
            int endIndex = curvePoints.Count - 1;

            // 寻找最接近的左右边界点
            for (int i = 0; i < curvePoints.Count; i++)
            {
                if (playerLevel >= curvePoints[i]._x)
                {
                    startIndex = i;

                    if (i < curvePoints.Count - 1 && playerLevel < curvePoints[i + 1]._x)
                    {
                        endIndex = i + 1;
                        break;
                    }
                }
            }

            AttackCooldownCurvePoint startPoint = curvePoints[startIndex];
            AttackCooldownCurvePoint endPoint = curvePoints[endIndex];

            // 使用线性插值计算值
            float t = Mathf.InverseLerp(startPoint._x, endPoint._x, playerLevel);
            float fx = Mathf.Lerp(startPoint._f_x_, endPoint._f_x_, t);

            return limitByMINMAX ? Mathf.Clamp(fx, min, max) : fx;
        }

        
        // public float CalculateValueAt(int playerLevel)
        // {
        //     if (curvePoints.Count == 0)
        //     {
        //         // Debug.LogWarning("Attack Cooldown Curve is empty. Returning default cooldown.");
        //         return 1.0f;
        //     }
        //
        //     float fx = curvePoints[0]._f_x_;
        //
        //     for (int i = 0; i < curvePoints.Count; i++)
        //     {
        //         AttackCooldownCurvePoint currentPoint = curvePoints[i];
        //
        //         if (playerLevel >= currentPoint._x)
        //         {
        //             // 如果玩家等级大于或等于当前点的等级
        //             if (i < curvePoints.Count - 1)
        //             {
        //                 AttackCooldownCurvePoint nextPoint = curvePoints[i + 1];
        //                 float t = Mathf.InverseLerp(currentPoint._x, nextPoint._x, playerLevel);
        //                 fx = Mathf.Lerp(currentPoint._f_x_, nextPoint._f_x_, t);
        //             }
        //             else
        //             {
        //                 fx = currentPoint._f_x_; // 如果玩家等级超过了最高定义的等级点，使用最后一个点的值
        //             }
        //         }
        //     }
        //
        //     return limitByMINMAX ? Mathf.Clamp(fx, min, max) : fx;
        // }
    }
}