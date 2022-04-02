using System.Collections;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class DataSerializer : MonoBehaviour, IDataManager
{
    public event Action<VehicleDataBaseStorage> DataLoaded;

    public void RequestData()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "VehicleDataBase.json");
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(ReadFromStreamingAssets(path));
        }
        else
        {
            OnDataLoaded(File.ReadAllText(path));
        }
    }
    IEnumerator ReadFromStreamingAssets(string path)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
        {
            yield return webRequest.SendWebRequest();
            OnDataLoaded(webRequest.downloadHandler.text);
        }
    }
    private void OnDataLoaded(string data)
    {
        DataBaseSaveContext dbContext = JsonUtility.FromJson<DataBaseSaveContext>(data);
        VehicleDataBaseStorage dataBaseStorage = DataBaseSaveContext.GenerateDataBase(dbContext);
        DataLoaded?.Invoke(dataBaseStorage);
    }
    public void SaveData(VehicleDataBaseStorage storage) => throw new NotImplementedException("Saving Data in RunTime is not ready yet");
}
