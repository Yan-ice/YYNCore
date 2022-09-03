using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    //�첽���ض���
    private AsyncOperation prog;

    // �첽���ؽ��Ȳ���
    int toProgress = 0;
    int showProgress = 0;

    // �첽�����¼�
    System.Action<float> loadingAction = null;
    System.Action loadBeforeAction = null;
    System.Action loadedEndAction = null;

    /// <summary>
    /// ͬ�����س���
    /// </summary>
    /// <param name="scenesName">��������</param>
    public void LoadScene(string scenesName)
    {

        SceneManager.LoadScene(scenesName);
    }

    /// <summary>
    /// �첽����
    /// </summary>
    /// <param name="scenesName">��������</param>
    /// <param name="loadingAction">�����е��¼�</param>
    /// <param name="loadBeforeAction">�첽����ǰ���¼�</param>
    /// <param name="loadedEndAction">�첽������ɺ���¼�</param>
    public void LoadSceneAsync(string scenesName, System.Action<float> loadingAction = null, System.Action loadBeforeAction = null, System.Action loadedEndAction = null)
    {
        // �¼���ֵ
        this.loadBeforeAction = loadBeforeAction;
        this.loadingAction = loadingAction;
        this.loadedEndAction = loadedEndAction;

        StartCoroutine(LoadingScene(scenesName));
    }

    /// <summary>
    /// �첽���ؽ���
    /// </summary>
    /// <returns></returns>
    public float ProgressLoadSceneAsync()
    {

        return showProgress;
    }

    /// <summary>
    /// Э�̼��س���
    /// </summary>
    /// <param name="scenesName">��������</param>
    /// <param name="loadingAction">�����е��¼�</param>
    /// <param name="loadBeforeAction">�첽����ǰ���¼�</param>
    /// <param name="loadedEndAction">�첽������ɺ���¼�</param>
    /// <returns></returns>
    private IEnumerator LoadingScene(string scenesName)
    {
        // �첽����ǰ���¼�
        if (this.loadBeforeAction != null)
        {
            this.loadBeforeAction.Invoke();
        }

        prog = SceneManager.LoadSceneAsync(scenesName);  //�첽���س���

        prog.allowSceneActivation = false;  //���������ɣ�Ҳ�����볡��

        toProgress = 0;
        showProgress = 0;

        //������һ�£�����������0.9
        while (prog.progress < 0.9f)
        {
            toProgress = (int)(prog.progress * 100);

            while (showProgress < toProgress)
            {
                showProgress++;

                // �첽�����е��¼�
                if (this.loadingAction != null)
                {
                    this.loadingAction.Invoke(showProgress);
                }
            }
            yield return new WaitForEndOfFrame(); //�ȴ�һ֡
        }
        //����0.9---1   ��ʵ0.9���Ǽ��غ��ˣ��������뵽������1  
        toProgress = 100;

        while (showProgress < toProgress)
        {
            showProgress++;

            // �첽�����е��¼�
            if (this.loadingAction != null)
            {
                this.loadingAction.Invoke(showProgress);
            }

            yield return new WaitForEndOfFrame(); //�ȴ�һ֡
        }

        yield return new WaitForEndOfFrame(); //�ȴ�һ֡

        prog.allowSceneActivation = true;  //���������ɣ����볡��

        // �첽������ɺ���¼�
        if (this.loadedEndAction != null)
        {
            this.loadedEndAction.Invoke();
        }
    }


}