using System;
using UnityEngine;

public class BloodMistController : MonoBehaviour
{
    public Material bloodMistMaterial; // 用于应用Shader的材质
    public State playerSt; // 您的玩家控制器
    private static readonly int PlayerHealth = Shader.PropertyToID("_PlayerHealth");

    private void Start()
    {
        playerSt = GameObject.Find("Player").GetComponent<State>();
    }

    private void Update()
    {
        if (bloodMistMaterial != null && playerSt != null)
        {
            // 获取玩家血量的百分比
            float playerHealth = playerSt.GetNormalizedHealth();

            // 将血量传递给Shader
            bloodMistMaterial.SetFloat(PlayerHealth, playerHealth);
        }
    }
}