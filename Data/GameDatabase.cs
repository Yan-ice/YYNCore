
using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

public class BasicInformation
{
    [PrimaryKey]
    public string key { get; set; }

    public int value { get; set; } = 0;
}


public class GameDatabase
{
    public static GameDatabase base_data = new GameDatabase("internalData.db");

    private SQLiteConnection m_internalDataConnection = null;
    private string m_dataFilePath = "internalData.db";

    public GameDatabase(string path)
    {
        LoadPath(path);
    }

    public void LoadPath(string path)
    {
        m_dataFilePath = path;
        if (m_internalDataConnection != null)
        {
            m_internalDataConnection.Close();
            m_internalDataConnection = null;
        }
    }

    public bool FileExist()
    {
        return FileMgr.Exists(m_dataFilePath);
    }

    public int GetData(string key)
    {
        LoadDatabase();
        BasicInformation binf = LoadObjects<BasicInformation>(_ => _.key == key).FirstOrDefault();
        if (binf != null)
        {
            return binf.value;
        }
        return 0;
    }
    public void SetData(string key, int value)
    {
        LoadDatabase();
        BasicInformation inf = new BasicInformation();
        inf.key = key;
        inf.value = value;
        SaveOrReplaceObject<BasicInformation>(inf);
    }
    public void Disconnect()
    {
        m_internalDataConnection.Close();
        m_internalDataConnection = null;
    }

    private void LoadDatabase()
    {
        if (m_internalDataConnection == null)
        {
            if (!FileMgr.Exists(m_dataFilePath))
            {
                Debug.Log("finding file in " + FileMgr.convertToAbsolute(m_dataFilePath));
                if(!FileMgr.CopyOutFromAsset(m_dataFilePath, false))
                {
                    Debug.Log("creating new file in " + FileMgr.convertToAbsolute(m_dataFilePath));
                    FileMgr.openFile(m_dataFilePath, false).Close();
                }
            }
            m_internalDataConnection = new SQLiteConnection(FileMgr.convertToAbsolute(m_dataFilePath), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }
    }

    public T LoadObject<T>(object primary_key) where T: new()
    {
        LoadDatabase();
        try
        {
            return m_internalDataConnection.Get<T>(primary_key);
        }
        catch (InvalidOperationException)
        {
            return default(T);
        }
    }

    public TableQuery<T> LoadObjects<T>(Expression<Func<T, bool>> expression) where T : new()
    {
        LoadDatabase();
        m_internalDataConnection.CreateTable<T>(CreateFlags.ImplicitPK);
        TableQuery<T> q = m_internalDataConnection.Table<T>().Where(expression);
        return q;
    }
    public void SaveOrReplaceObject<T>(T obj) where T : new()
    {
        LoadDatabase();
        m_internalDataConnection.CreateTable<T>(CreateFlags.None);
        m_internalDataConnection.InsertOrReplace(obj);
    }
    public T SaveObjectAsNew<T>(T obj) where T : new()
    {
        LoadDatabase();
        m_internalDataConnection.CreateTable<T>(CreateFlags.None);

        int row_num = m_internalDataConnection.Table<T>().Count();
        m_internalDataConnection.Insert(obj);
        return m_internalDataConnection.Table<T>().Skip(row_num).First();
    }

    public void DeleteObjects<T>(Expression<Func<T, bool>> expression) where T : new()
    {
        LoadDatabase();
        TableQuery<T> objs = LoadObjects(expression);
        foreach (T obj in objs)
        {
            m_internalDataConnection.Delete(obj);

        }
    }
    public void DeleteObject<T>(T obj)
    {
        LoadDatabase();
        m_internalDataConnection.Delete(obj);
    }
}

