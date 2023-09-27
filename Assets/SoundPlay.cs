using System.Collections.Generic;
using Behavior;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundPlay : StateMachineBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;
    [SerializeField] private bool stopOnExit;
    [SerializeField] private float delay;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioClips.Count == 0)
        {
            Debug.LogError("没有指定音频剪辑，请添加音频剪辑到列表中。");
            return;
        }

        
        
        if (audioSource == null)
        {
            audioSource = PlayerController.Instance.GetComponent<AudioSource>();
        }

        // 随机选择一个音频剪辑
        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip randomClip = audioClips[randomIndex];

        // 将随机选中的音频剪辑给音源
        
        if (delay == 0f)
        {
            audioSource.PlayOneShot(randomClip);
        }else
        {
            audioSource.clip = randomClip;
            audioSource.PlayDelayed(delay);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 停止播放音频
        if(stopOnExit) audioSource.Stop();
    }
}