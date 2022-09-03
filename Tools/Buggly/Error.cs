using UnityEngine;
using UnityEngine.UI;

class Error : UIWindow
{
    #region 脚本工具生成的代码
    private Button m_btnCopy;
    private Button m_btnBack;
    private Text m_textErrorInf;
    /*该方法会在初始化前被UIWindow自动调用。*/
    protected override void ComponentInit()
    {
        m_btnCopy = FindChildComponent<Button>("window/m_btnCopy");
        m_btnBack = FindChildComponent<Button>("window/m_btnBack");
        m_textErrorInf = FindChildComponent<Text>("window/m_textErrorInf");
        m_btnCopy.onClick.AddListener(OnClickCopyBtn);
        m_btnBack.onClick.AddListener(OnClickBackBtn);
    }
    #endregion

    #region 事件
    private void OnClickCopyBtn()
    {
        TextEditor te = new TextEditor();
        te.text = m_textErrorInf.text;
        te.SelectAll();
        te.Copy();
        te.Copy();
    }
    private void OnClickBackBtn()
    {
        Hide();
    }
    #endregion


    public void setContent(string errormsg)
    {
        m_textErrorInf.text = errormsg;
    }
    protected override void OnDestroy()
    {
        
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnInit(GameObject m_go)
    {
       
    }

    protected override void OnShow()
    {
        
    }


    protected override int SortOrder()
    {
        return 10004;
    }
}
