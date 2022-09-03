using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    //异步加载对象
    private AsyncOperation prog;

    // 异步加载进度参数
    int toProgress = 0;
    int showProgress = 0;

    // 异步加载事件
    System.Action<float> loadingAction = null;
    System.Action loadBeforeAction = null;
    System.Action loadedEndAction = null;

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="scenesName">场景名称</param>
    public void LoadScene(string scenesName)
    {

        SceneManager.LoadScene(scenesName);
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="scenesName">场景名称</param>
    /// <param name="loadingAction">加载中的事件</param>
    /// <param name="loadBeforeAction">异步加载前的事件</param>
    /// <param name="loadedEndAction">异步加载完成后的事件</param>
    public void LoadSceneAsync(string scenesName, System.Action<float> loadingAction = null, System.Action loadBeforeAction = null, System.Action loadedEndAction = null)
    {
        // 事件赋值
        this.loadBeforeAction = loadBeforeAction;
        this.loadingAction = loadingAction;
        this.loadedEndAction = loadedEndAction;

        StartCoroutine(LoadingScene(scenesName));
    }

    /// <summary>
    /// 异步加载进度
    /// </summary>
    /// <returns></returns>
    public float ProgressLoadSceneAsync()
    {

        return showProgress;
    }

    /// <summary>
    /// 协程加载场景
    /// </summary>
    /// <param name="scenesName">场景名称</param>
    /// <param name="loadingAction">加载中的事件</param>
    /// <param name="loadBeforeAction">异步加载前的事件</param>
    /// <param name="loadedEndAction">异步加载完成后的事件</param>
    /// <returns></returns>
    private IEnumerator LoadingScene(string scenesName)
    {
        // 异步加载前的事件
        if (this.loadBeforeAction != null)
        {
            this.loadBeforeAction.Invoke();
        }

        prog = SceneManager.LoadSceneAsync(scenesName);  //异步加载场景

        prog.allowSceneActivation = false;  //如果加载完成，也不进入场景

        toProgress = 0;
        showProgress = 0;

        //测试了一下，进度最大就是0.9
        while (prog.progress < 0.9f)
        {
            toProgress = (int)(prog.progress * 100);

            while (showProgress < toProgress)
            {
                showProgress++;

                // 异步加载中的事件
                if (this.loadingAction != null)
                {
                    this.loadingAction.Invoke(showProgress);
                }
            }
            yield return new WaitForEndOfFrame(); //等待一帧
        }
        //计算0.9---1   其实0.9就是加载好了，真正进入到场景是1  
        toProgress = 100;

        while (showProgress < toProgress)
        {
            showProgress++;

            // 异步加载中的事件
            if (this.loadingAction != null)
            {
                this.loadingAction.Invoke(showProgress);
            }

            yield return new WaitForEndOfFrame(); //等待一帧
        }

        yield return new WaitForEndOfFrame(); //等待一帧

        prog.allowSceneActivation = true;  //如果加载完成，进入场景

        // 异步加载完成后的事件
        if (this.loadedEndAction != null)
        {
            this.loadedEndAction.Invoke();
        }
    }


}