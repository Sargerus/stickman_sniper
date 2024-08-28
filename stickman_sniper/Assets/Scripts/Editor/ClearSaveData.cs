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

        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets\\Scripts\\CustomizationSystem" });
        for (int i = 0; i < guids.Length; i++)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(guids[i]);
            if (asset is WeaponCharacteristicsContainer wcc)
            {
                wcc.Config.ForEach(g => g.CurrentCustomizationData = g.DefaultCustomizationData);
            }
        }

        Debug.Log("-==Data cleared==-");
    }
}
