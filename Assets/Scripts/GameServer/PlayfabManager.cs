using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class PlayfabManager : Singleton<PlayfabManager>
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text loadingText;

    public override void Awake()
    {
        MakeSingleton(true);
    }

    public override void Start()
    {
        base.Start();
        LoginAndCheckUpdateData();
    }
    private void CheckNetworkAndUpdateLevelsData()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result => {
                if (result.Data == null || !result.Data.ContainsKey("LevelVersion")) Debug.Log("No Exist Version of Level Data");
                else
                {
                    if (result.Data.TryGetValue("LevelVersion", out string versionStr))
                    {
                        int serverLevelVersion = int.Parse(versionStr);

                        if (serverLevelVersion > Pref.levelVersion)
                        {
                            Debug.Log("There is a new version, please update!");
                            Pref.levelVersion = serverLevelVersion;
                            // Active screen update...
                            StartCoroutine(LoadDataCoroutineAndUpdateDataToJsonLocal());
                        }
                        else
                        {
                            Debug.Log("There is no new version!");
                            // Load Data From Local
                            //LoadLocalLevelData();
                            return;
                        }
                    }
                }
            },
            error => {
                Debug.Log("Got error getting titleData:");
                Debug.Log(error.GenerateErrorReport());
            }
        );}
        else
        {
            Debug.Log("Login failed!");
            return;
        }
    }
    private IEnumerator LoadDataCoroutineAndUpdateDataToJsonLocal()
    {
        if(loadingScreen == null && progressBar == null)
        {
            yield return null;
        }

        loadingScreen.SetActive(true);
        progressBar.fillAmount = 0;
        loadingText.text = "Loading... 0%";

        bool isDataLoaded = false;

        PlayFabClientAPI.GetTitleData(new PlayFab.ClientModels.GetTitleDataRequest(),
            result => {
                if (result.Data == null || !result.Data.ContainsKey("Levels")) Debug.Log("No Levels");
                else
                {
                    string levelDataJson = result.Data["Levels"];
                    // Parse JSON thành object
                    LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);
                    UpdateLevelsDataToJsonFile(levelDataJson);
                    Debug.Log("Loaded level successfull! NoLevels: " + levelData.levelScriptableDatas.Count);
                    isDataLoaded = true;
                }
            },
            error => {
                Debug.Log("Got error getting titleData:");
                Debug.Log(error.GenerateErrorReport());
                isDataLoaded = true;
            }
        );

        float progress = 0;
        while (!isDataLoaded)
        {
            progress += Time.deltaTime * 0.5f; 
            progressBar.fillAmount = Mathf.Clamp01(progress); 
            loadingText.text = $"Loading... {Mathf.FloorToInt(progress * 100)}%";

            yield return null; 
        }

        progressBar.fillAmount = 1;
        loadingText.text = "Loading... 100%";

        yield return new WaitForSeconds(2f);
        loadingScreen.SetActive(false);
    }
    public void UpdateLevelsToScritableobject(List<LevelScriptableData> levelList)
    {

        LevelScriptableData[] allLevels = Resources.LoadAll<LevelScriptableData>("");

        foreach (LevelScriptableData levelData in levelList)
        {
            // Tìm ScriptableObject tương ứng với levelID
            LevelScriptableData so = FindLevelSOByName(allLevels, levelData.name.ToString());
            if (so != null)
            {
                // Add method Update Data In LevelScriptableData script
                so.UpdateData(levelData);  // Cập nhật dữ liệu
                Debug.Log($"Level {so.name} đã được cập nhật.");
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy ScriptableObject cho levelID: {levelData.name}");
                // Create Instance For New Scritableobject
            }
        }
       
    }

    private LevelScriptableData FindLevelSOByName(LevelScriptableData[] allLevels, string levelName)
    {
        foreach (LevelScriptableData level in allLevels)
        {
            if (level.name == levelName)
                return level;
        }
        return null;
    }
    private void LoginAndCheckUpdateData()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,
            result => {
                Debug.Log("Successfull login/ account create!");
                CheckNetworkAndUpdateLevelsData();
            },
            error =>
            {
                Debug.Log("Error while login in/ creating account!");
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public void UpdateLevelsDataToJsonFile(string levelDataString)
    {
        if(string.IsNullOrEmpty(levelDataString))
        {
            Debug.Log("Level Data load from Server is Null or Empty!");
            return;
        }

        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/LevelData.json", levelDataString);
            Debug.Log("<color=green>[Level Data] Saved.</color>");
        }
        catch (System.Exception e)
        {
            Debug.Log("Error Saving data" + e);
            throw;
        }
    }
    public void UploadLevelsDataToServer()
    {
        string levelJson = SaveLoadData.Ins.LoadStringLevelsDataFromLocal();
        if(levelJson == null || levelJson == "")
        {
            return;
        }

        PlayFabServerAPI.SetTitleData(
            new PlayFab.ServerModels.SetTitleDataRequest
            {
                Key = "Levels",
                Value = levelJson
            },
            result => Debug.Log("Set titleData successful"),
            error => {
                Debug.Log("Got error setting titleData:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }
}
