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

    private float _Sound_volume = 1f;

    public static float Sound_volume
    {
        set
        {
            Instance._Sound_volume = value;
        }
        get
        {
            return Instance._Sound_volume;
        }
    }

    private float _BGM_volume = 1f;

    public static float BGM_volume { 
        set {
            Instance._BGM_volume = value;
            Instance.bgm_source.volume = value;
        }
        get
        {
            return Instance._BGM_volume;
        }
    }

    List<AudioSource> audioSourceList = new List<AudioSource>();
    AudioSource bgm_source;
    private ResourcesLoader loader;
    protected override void onInit(){
        loader = new ResourcesLoader("audio");

        gameObject.AddComponent<AudioListener>();

        bgm_source = gameObject.AddComponent<AudioSource>();
        bgm_source.loop = true;
        //bgm的播放器

        for(int a = 0; a < 20; a++)
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
    public AudioSource Play(AudioClip audio_source) {

        for (int i = 0; i < audioSourceList.Count; i++) {
            AudioSource audioSource = audioSourceList[i];
            if (!audioSource.isPlaying) {
                audioSource.clip = audio_source;
                audioSource.volume = _Sound_volume;
                audioSource.Play();
                return audioSource;
            }
        }
        Debug.LogWarning("There is no available AudioSource.");
        return null;
    }

    /// <summary>
    /// 播放一个声音，将在Resources/Audio/Sound寻找资源文件。
    /// </summary>
    /// <param name="soundName"></param>
    public AudioSource Play(string audio_source)
    {
        AudioClip cl = loader.LoadResource<AudioClip>("sound/"+audio_source);
        return Play(cl);
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
    public void PlayBGM(AudioClip audioSource)
    {
        if (bgm_source.isPlaying)
        {
            bgm_source.Stop();
        }
        bgm_source.volume = BGM_volume;
        bgm_source.clip = audioSource;
        bgm_source.Play();
    }

    /// <summary>
    /// 播放一个bgm，将在Resources/Audio/Sound寻找资源文件。
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBGM(string audio_source)
    {
        AudioClip cl = loader.LoadResource<AudioClip>("bgm/" + audio_source);
        PlayBGM(cl);
    }

    public IEnumerable<int> AnimStopBGM(int duration)
    {
        for(int a = 0; a < duration; a++)
        {
            bgm_source.volume = (duration-a) / (float)duration * BGM_volume;
            yield return 0;
        }
        bgm_source.Stop();
        
    }
    public IEnumerable<int> AnimPlayBGM(AudioClip audioSource, int duration)
    {

        bgm_source.volume = 0;
        bgm_source.clip = audioSource;
        bgm_source.Play();
        for (int a = 0; a < duration; a++)
        {
            bgm_source.volume = a / (float)duration * BGM_volume;
            yield return 0;
        }
    }

    public IEnumerable<int> AnimPlayBGM(string audioSource, int duration)
    {

        bgm_source.volume = 0;
        bgm_source.clip = loader.LoadResource<AudioClip>("bgm/" + audioSource);
        bgm_source.Play();
        for (int a = 0; a < duration; a++)
        {
            bgm_source.volume = a / (float)duration * BGM_volume;
            yield return 0;
        }
    }

    /// <summary>
    /// 当前正在播放的bgm停止。
    /// </summary>
    public void StopBGM()
    {
        bgm_source.Stop();
    }
}