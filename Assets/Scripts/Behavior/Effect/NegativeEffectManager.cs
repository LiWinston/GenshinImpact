using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Behavior.Effect
{
    public class NegativeEffectManager : MonoBehaviour
    {
        [SerializeField] private GameObject statusBarPrefab;
        [SerializeField] private Transform statusBarParent;
        [SerializeField] private Transform statusBarStartPos;
        [SerializeField] private float statusBarSpacing = 0.05625f;
        [SerializeField] private float statusBarOffset = 0.01f;
        [SerializeField] private Vector3 startPosOffset = new Vector3(0f, 0f, 0f);
        
        private List<NegativeEffectBarUI> _statusBars;
        

        public void Awake(){
            _statusBars = new List<NegativeEffectBarUI>();
        }

        // 创建并初始化负面效果状态条
        public void CreateEffectBar(string effectType, Color fillColor, float duration)
        {
            NegativeEffectBarUI statusBarToRestart = _statusBars.Find(statusBar => statusBar.EffectType == effectType);
            if(statusBarToRestart != null){
                statusBarToRestart.Initialize(effectType, fillColor, duration);
                AdjustStatusBarPositions();
                return;
            }
            GameObject statusBarObj = Instantiate(statusBarPrefab, statusBarParent);
            // NegativeEffectBarUI statusBar = statusBarObj.GetComponent<NegativeEffectBarUI>();
            NegativeEffectBarUI statusBar = Find.FindDeepChild(statusBarObj.transform, "NegativeEffectBar").GetComponent<NegativeEffectBarUI>();
            statusBar.Initialize(effectType, fillColor, duration);
            _statusBars.Add(statusBar);

            // 订阅事件，当Bar到期时从列表中移除
            statusBar.OnEffectBarExpired += RemoveExpiredEffectBar;

            // 调整状态条的位置
            AdjustStatusBarPositions();
        }

        // 中止负面效果
        public void StopEffect(string effectType)
        {
            NegativeEffectBarUI statusBarToRemove = _statusBars.Find(statusBar => statusBar.EffectType == effectType);

            if (statusBarToRemove != null)
            {
                _statusBars.Remove(statusBarToRemove);
                Destroy(statusBarToRemove.gameObject);

                // 调整状态条的位置
                AdjustStatusBarPositions();
            }
        }

        private void RemoveExpiredEffectBar(string effectType)
        {
            NegativeEffectBarUI statusBarToRemove = _statusBars.Find(statusBar => statusBar.EffectType == effectType);

            if (statusBarToRemove != null)
            {
                _statusBars.Remove(statusBarToRemove);
                Destroy(statusBarToRemove.gameObject);

                // 调整状态条的位置
                AdjustStatusBarPositions();
            }
        }
        
        // 调整状态条的位置
        private void AdjustStatusBarPositions()
        {
            for (int i = 0; i < _statusBars.Count; i++)
            {
                Vector3 newPosition = statusBarStartPos.position + startPosOffset;
                newPosition.y -= (statusBarSpacing + i * (statusBarSpacing + statusBarOffset));
                _statusBars[i].transform.position = newPosition;
            }
        }
    }
}