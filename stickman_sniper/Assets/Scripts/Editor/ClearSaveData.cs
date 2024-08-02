using System.IO;
using UnityEditor;
using UnityEngine;

public class ClearSaveData : Editor
{
    [MenuItem("Tools/ClearGameData", false, 5)]
    public static void ClearData()
    {
        System.IO.DirectoryInfo di = new DirectoryInfo(Application.dataPath + "\\YandexGame\\WorkingData\\Editor");
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        Debug.Log("-==Data cleared==-");
    }
}
