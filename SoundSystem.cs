using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 他应该有一个AudioSource池（有好几个source），支持同时有多个音效播放。
/// 不能在一个播放时停止另一个
/// </summary>
public class SoundSystem : Singleton<SoundSystem>{

    AudioSource[] audioSourceList;
    public SoundSystem(){
        GameObject sd_obj = Resources.Load<GameObject>("AudioSystem");
        sd_obj = GameObject.Instantiate(sd_obj);
        GameObject.DontDestroyOnLoad(sd_obj);
        //在这里拿到自己身上的AudioSource
        audioSourceList = sd_obj.GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// 播放一个声音。
    /// </summary>
    /// <param name="soundName"></param>
    public void Play(string soundName) {
        AudioClip audio_source = ResourceManager.Instance.LoadAudio("sound", soundName);
        bool successful = false;
        for (int i = 0; i < audioSourceList.Length; i++) {
            AudioSource audioSource = audioSourceList[i];
            if (!audioSource.isPlaying) {
                audioSource.clip = audio_source;
                audioSource.Play();
                successful = true;
                break;
            }
        }
        if (!successful) {
            Debug.LogWarning("There is no available AudioSource.");
        }
    }


    /// <summary>
    /// 当前正在播放的所有声音停止。
    /// </summary>
    public void StopAll() {
        for (int i = 0; i < audioSourceList.Length; i++) {
            audioSourceList[i].Stop();
        }
    }
}