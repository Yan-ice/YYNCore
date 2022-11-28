using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 他应该有一个AudioSource池（有好几个source），支持同时有多个音效播放。
/// 不能在一个播放时停止另一个
/// </summary>
public class AudioSystem : MonoSingleton<AudioSystem>{

    List<AudioSource> audioSourceList = new List<AudioSource>();
    AudioSource bgm_source;

    protected override void onInit(){
        gameObject.AddComponent<AudioListener>();

        bgm_source = gameObject.AddComponent<AudioSource>();
        bgm_source.loop = true;
        //bgm的播放器

        for(int a = 0; a < 5; a++)
        {
            audioSourceList.Add(gameObject.AddComponent<AudioSource>());
        }
        //sound的播放器
        
        GameObject.DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 播放一个声音。
    /// </summary>
    /// <param name="soundName"></param>
    public void Play(string soundName) {
        AudioClip audio_source = ResourceManager.Instance.LoadAudio("sound", soundName);
        bool successful = false;
        for (int i = 0; i < audioSourceList.Count; i++) {
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
    public void StopAllSound() {
        for (int i = 0; i < audioSourceList.Count; i++) {
            audioSourceList[i].Stop();
        }
    }

    /// <summary>
    /// 播放一个bgm。
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        AudioClip audioSource = ResourceManager.Instance.LoadAudio("BGM", bgmName);
        if (bgm_source.isPlaying)
        {
            bgm_source.Stop();
        }
        bgm_source.clip = audioSource;
        bgm_source.Play();
    }

    /// <summary>
    /// 当前正在播放的bgm停止。
    /// </summary>
    public void StopBGM()
    {
        bgm_source.Stop();
    }
}