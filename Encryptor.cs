
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

class Encryptor
{
    public static bool enableEncrypting = true;

    static string m_key = "yatyat12";
    static string m_iv = "12yatyat";


    /// <summary>
    /// 发送加密的数据
    /// </summary>
    /// <param name="socket">发送者Socket</param>
    /// <param name="command">发送的指令</param>
    public static void SendEncryptMessage(Socket socket, string command)
    {
            string cutend = command.Substring(0, command.Length - 1);//去掉最后的$
            Log.Message("发送:" + command);
            byte[] utf8 = Encoding.UTF8.GetBytes(DESEncrypt(cutend));
            //Log.Message("发送转码:" + Encoding.UTF8.GetString(utf8) + "$");
            socket.Send(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(utf8) + "$"));//发送TCP信息
    }

    #region DESCryptoService对称加密
    /// <summary>   
    /// DESCryptoService对称加密   
    /// </summary>   
    /// <param name="strSource">需要加密的字符串</param>   
    /// <returns>DESCryptoService对称加密后的字符串</returns>   
    public static string DESEncrypt(string strSource)
    {
        if (!enableEncrypting)
        {
            return strSource;
        }
        //把字符串放到byte数组中   
        byte[] bytIn = System.Text.Encoding.UTF8.GetBytes(strSource);
        //建立加密对象的密钥和偏移量        
        byte[] key = { 55, 103, 246, 79, 36, 99, 167, 3 };//定义密钥      
        byte[] iv = { 102, 16, 93, 156, 78, 4, 218, 32 };//定义偏移量   

        key = System.Text.Encoding.UTF8.GetBytes(m_key);
        iv = System.Text.Encoding.UTF8.GetBytes(m_iv);

        //实例DES加密类   
        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = key;
        mobjCryptoService.IV = iv;
        ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
        //实例MemoryStream流加密密文件   
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
        cs.Write(bytIn, 0, bytIn.Length);
        cs.FlushFinalBlock();
        return Convert.ToBase64String(ms.ToArray());
    }
    #endregion

    #region DESCryptoService对称解密
    /// <summary>   
    /// DESCryptoService对称解密   
    /// </summary>   
    /// <param name="Source">需要解密的字符串</param>   
    /// <returns>DESCryptoService对称解密后的字符串</returns>   
    public static string DESDecrypt(string Source)
    {
        if (!enableEncrypting)
        {
            return Source;
        }
        //Log.Message("解码:"+Source);
        //将解密字符串转换成字节数组   
        byte[] bytIn = Convert.FromBase64String(Source);
        //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同   
        byte[] key = { 55, 103, 246, 79, 36, 99, 167, 3 };//定义密钥   
        byte[] iv = { 102, 16, 93, 156, 78, 4, 218, 32 };//定义偏移量   

        key = System.Text.Encoding.UTF8.GetBytes(m_key);
        iv = System.Text.Encoding.UTF8.GetBytes(m_iv);
        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = key;
        mobjCryptoService.IV = iv;
        //实例流进行解密   
        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
        ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
        StreamReader strd = new StreamReader(cs, Encoding.UTF8);
        return strd.ReadToEnd();
    }
    #endregion
}   


