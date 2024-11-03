using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System.Threading;
using System;
using System.Threading.Tasks;

public class PlayfabManager : Singleton<PlayfabManager>
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject updateDialog;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text loadingText;
    [SerializeField] private Button updateBtn;
    [SerializeField] private Button cancelBtn;


    public override void Awake()
    {
        MakeSingleton(true);
    }

    public override void Start()
    {
        base.Start();
        if (IsConnectedToInternet())
            LoginAndCheckUpdateData();
        else
            LoadDataFromJsonLocal();
    }
    public bool IsConnectedToInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
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

    private void CheckNetworkAndUpdateLevelsData()
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
                        if (updateBtn == null || cancelBtn == null || updateDialog == null)
                            return;
                        updateDialog.SetActive(true);
                        updateBtn.onClick.RemoveAllListeners();
                        updateBtn.onClick.AddListener(() =>
                        {
                            updateDialog.SetActive(false);
                            Debug.Log("There is a new version, please update!");
                            Pref.levelVersion = serverLevelVersion;
                            // Active screen update...
                            StartCoroutine(LoadDataCoroutineAndUpdateDataToJsonLocal());
                        });
                        cancelBtn.onClick.RemoveAllListeners();
                        cancelBtn.onClick.AddListener(() => {
                            updateDialog.SetActive(false);
                            LoadDataFromJsonLocal();
                            });
                    }
                    else
                    {
                        Debug.Log("There is no new version!");
                        // Load Data From Local
                        LoadDataFromJsonLocal();
                        return;
                    }
                }
            }
        },
        error => {
            Debug.Log("Got error getting titleData:");
            Debug.Log(error.GenerateErrorReport());
        });
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

                    UpdateLevelsDataToJsonFile(levelData);

                    LoadDataFromJsonLocal();
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
        loadingText.text = "Update Successfull!";

        yield return new WaitForSeconds(2.5f);
        loadingScreen.SetActive(false);
    }

    private void LoadDataFromJsonLocal()
    {
        if (AddressableManager.Ins)
            StartCoroutine(AddressableManager.Ins.LoadSprites("subject1"));

        if (LevelSystemManager.Ins)
            LevelSystemManager.Ins.InitData();
    }

    public void UpdateLevelsDataToJsonFile(LevelData levelDataFromServer)
    {
        if (levelDataFromServer == null || levelDataFromServer.levelScriptableDatas == null || levelDataFromServer.levelScriptableDatas.Count == 0)
        {
            Debug.Log("Level Data load from Server is Null or Empty!");
            return;
        }
        List<LevelObjectData> listLevelFromLocal = SaveLoadData.Ins.LoadListLevelsDataFromLocal();

        foreach (LevelObjectData level in listLevelFromLocal)
        {
            LevelObjectData levelTemp = levelDataFromServer.levelScriptableDatas.Find(item => item.levelId == level.levelId && level.unlocked == true);
            if (levelTemp != null)
            {
                levelTemp.unlocked = level.unlocked;
                levelTemp.startArchived = level.startArchived;
            }
        }

        string levelDataString = JsonConvert.SerializeObject(levelDataFromServer, Formatting.Indented);
        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/LevelData.json", levelDataString);
            Debug.Log("<color=green>New Version of [Level Data] is Updated.</color>");
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


    /* if use ScriptableObject
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
                //so.UpdateData(levelData);  // Cập nhật dữ liệu
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
     */
}
