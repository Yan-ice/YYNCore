using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 资源加载类统一接口。
/// 它有三种可能实现：Resources加载，AB包加载，mod加载。
/// </summary>
public interface ResourceLoaderInterface
{
    /// <summary>
    /// 返回该加载器(包)的名字。
    /// </summary>
    /// <returns></returns>
    public string PackName();

    /// <summary>
    /// 在指定路径读取特定类型资源。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="t">资源类型</param>
    /// <returns>加载的对象</returns>
    public object LoadResource(string path, Type t);

    /// <summary>
    /// 在指定路径读取特定类型资源。
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns>加载的对象</returns>
    public T LoadResource<T>(string path);

    /// <summary>
    /// 在指定路径保存资源。
    /// 注意：如果t是一个JSON可序列化的类，您必须加载文本类型资源，然后将文本反序列化。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="resource">资源对象</param>
    /// <returns>所有资源</returns>
    public void SaveResource(string path, object resource);

    /// <summary>
    /// 返回所有可访问资源的路径的集合。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Manifest();
}