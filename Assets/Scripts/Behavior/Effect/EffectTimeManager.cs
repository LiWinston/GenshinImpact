using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Behavior.Effect
{
    public class EffectTimeManager : MonoBehaviour
    {
        [FormerlySerializedAs("statusBarPrefab")] [SerializeField] private GameObject effectBarPrefab;
        [FormerlySerializedAs("statusBarParent")] [SerializeField] private Transform effectBarParent;
        [FormerlySerializedAs("statusBarStartPos")] [SerializeField] private Transform effectBarStartPos;
        [FormerlySerializedAs("statusBarSpacing")] [SerializeField] private float effectBarSpacing = 0.05625f;
        [FormerlySerializedAs("statusBarOffset")] [SerializeField] private float effectBarOffset = 0.01f;
        [SerializeField] private Vector3 startPosOffset = new Vector3(0f, 0f, 0f);
        [SerializeField] private Vector3 transformNormalizer = new Vector3(1f, 1f, 1f);
        
        internal List<EffectTimeBarUI> _statusBars;
        

        public void Awake(){
            _statusBars = new List<EffectTimeBarUI>();
        }

        // 创建并初始化效果状态条
        public void CreateEffectBar(string effectType, Color fillColor, float duration)
        {
            EffectTimeBarUI effectTimeBarToRestart = _statusBars.Find(statusBar => statusBar.EffectType == effectType);
            if(effectTimeBarToRestart != null){
                effectTimeBarToRestart.Initialize(effectType, fillColor, duration);
                AdjustStatusBarPositions();
                return;
            }
            GameObject effectBarObj = Instantiate(effectBarPrefab, effectBarParent);
            
            // 设置生成的目标的尺寸
            effectBarObj.transform.localScale = transformNormalizer;
            
            // EffectTimeBarUI effectTimeBar = statusBarObj.GetComponent<EffectTimeBarUI>();
            // EffectTimeBarUI effectTimeBar = Find.FindDeepChild(effectBarObj.transform, "EffectBar").GetComponent<EffectTimeBarUI>();
            EffectTimeBarUI effectTimeBar = effectBarObj.transform.Find("EffectBar").GetComponent<EffectTimeBarUI>();
            effectTimeBar.Initialize(effectType, fillColor, duration);
            _statusBars.Add(effectTimeBar);

            // 订阅事件，当Bar到期时从列表中移除
            effectTimeBar.OnEffectBarExpired += StopEffect;

            // 调整状态条的位置
            AdjustStatusBarPositions();
        }

        // 中止效果
        public void StopEffect(string effectType)
        {
            EffectTimeBarUI effectTimeBarToRemove = _statusBars.Find(statusBar => statusBar.EffectType == effectType);

            if (effectTimeBarToRemove != null)
            {
                effectTimeBarToRemove.OnEffectBarExpired -= StopEffect;
                _statusBars.Remove(effectTimeBarToRemove);
                Destroy(effectTimeBarToRemove.transform.parent.gameObject);

                // 调整状态条的位置
                AdjustStatusBarPositions();
            }
        }
        
        
        // 调整状态条的位置
        private void AdjustStatusBarPositions()
        {
            for (int i = 0; i < _statusBars.Count; i++)
            {
                Vector3 newPosition = effectBarStartPos.position + startPosOffset;
                newPosition.y -= (effectBarSpacing + i * (effectBarSpacing + effectBarOffset));
                _statusBars[i].transform.position = newPosition;
            }
        }
    }
}