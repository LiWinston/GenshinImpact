using System;
using System.Collections.Generic;
using Behavior;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager instance;

    public static SoundEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundEffectManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("SoundEffectManager");
                    instance = managerObject.AddComponent<SoundEffectManager>();
                }
            }
            return instance;
        }
    }

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // 在该对象上查找或添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 播放单个音频文件
    public void PlaySound([NotNull] string audioFileName, GameObject targetObject = null){
        if (audioFileName == null) throw new ArgumentNullException(nameof(audioFileName));
        if (targetObject == null)
        {
            audioSource.clip = Resources.Load<AudioClip>(audioFileName);
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.Log("找不到音频文件: " + audioFileName);
            }
        }
        else
        {
            AudioSource targetAudioSource = targetObject.GetComponent<AudioSource>();
            if (targetAudioSource == null)
            {
                targetAudioSource = targetObject.AddComponent<AudioSource>();
            }

            targetAudioSource.clip = Resources.Load<AudioClip>(audioFileName);
            if (targetAudioSource.clip != null)
            {
                targetAudioSource.Play();
            }
            else
            {
                Debug.Log("找不到音频文件: " + audioFileName);
            }
        }
    }

    // 随机播放多个音频文件
    public void PlaySound([NotNull] List<string> audioFileNames, GameObject targetObject = null)
    {
        if (audioFileNames == null) throw new ArgumentNullException(nameof(audioFileNames));
        
        if (targetObject == null)
        {
            targetObject = PlayerController.Instance.gameObject;
            string randomAudioFileName = audioFileNames[Random.Range(0, audioFileNames.Count)];

            PlaySound(randomAudioFileName, targetObject);
        }
        else
        {
            AudioSource targetAudioSource = targetObject.GetComponent<AudioSource>();
            if (targetAudioSource == null)
            {
                targetAudioSource = targetObject.AddComponent<AudioSource>();
            }
            string randomAudioFileName = audioFileNames[Random.Range(0, audioFileNames.Count)];

            PlaySound(randomAudioFileName, targetObject);
        }

        
    }
}
