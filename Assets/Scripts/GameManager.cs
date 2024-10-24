using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GameManager : Singleton<GameManager>
{
    public int timeLimit;
    private List<MatchItem> matchItems;
    public MatchItemUI itemUIPb;

    public Transform gridRoot;
    public GridLayoutGroup gridLayoutGroup; 
    public RectTransform gridRectTransform; 

    public GameState state;
    private List<MatchItem> m_matchItemsCopy;
    public List<MatchItemUI> m_matchItemUIs;
    private List<MatchItemUI> m_answers;
    public float m_timeCounting;
    private int m_totalMatchItem;
    private int m_totalMoving;
    private int m_rightMoving;
    private bool m_isAnswerChecking;
    private List<LevelScriptableData> m_listLevelScriptsData;

    [SerializeField] private SpriteLoader m_spriteLoader; 
    [SerializeField] public GameObject messImg;

    public int TotalMoving { get => m_totalMoving;}
    public int RightMoving { get => m_rightMoving;}

    public override void Awake()
    {
        MakeSingleton(false);
    }
    public override void Start()
    {
        base.Start();
        
        if(SpriteLoader.Ins)
            m_spriteLoader = SpriteLoader.Ins;

        m_spriteLoader.OnSpritesLoaded += GenerateMatchItem;

        m_listLevelScriptsData = LevelSystemManager.Ins.LevelData.levelScriptableDatas;

        if (m_listLevelScriptsData.Count <= 0 || m_listLevelScriptsData == null)
        {
            LevelScriptableData[] resourcesLevel = Resources.LoadAll<LevelScriptableData>("");

            if (resourcesLevel != null && resourcesLevel.Length > 0)
            {
                m_listLevelScriptsData = new List<LevelScriptableData>(resourcesLevel);
            }
            else
            {
                Debug.Log("No ScriptableObjects found in Resources");
            }
        }
        // xử lý logic load sprite từ folder
        m_matchItemsCopy = new List<MatchItem>();

        m_matchItemUIs = new List<MatchItemUI>();
        m_answers = new List<MatchItemUI>();
        state = GameState.Starting;
        timeLimit = m_listLevelScriptsData[LevelSystemManager.Ins.CurrentLevel].timeLimit;
        m_timeCounting = timeLimit;
        StartCoroutine(m_spriteLoader.LoadSprites("subject1"));
    }

    private void Update()
    {
        if (state != GameState.Playing) return;

        m_timeCounting -= Time.deltaTime;
        if(m_timeCounting < 0 && state != GameState.Timeout)
        {
            state = GameState.Timeout;
            m_timeCounting = 0;
            if (DialogManager.Ins)
                DialogManager.Ins.timeoutDialog.Show(true);
            if (AudioController.Ins)
                AudioController.Ins.PlaySound(AudioController.Ins.timeOut);
            Debug.Log("Timeout");
        }

        if (DialogManager.Ins)
            DialogManager.Ins.UpdateTimeBar((float)m_timeCounting, (float)timeLimit);
    }

    public void PlayGame()
    {
        StartCoroutine(m_spriteLoader.LoadSprites("subject1"));
    }
    public List<Sprite> GetRandomSprites(List<Sprite> inputList, int count)
    {
        // Kiểm tra nếu count lớn hơn số lượng sprite trong inputList
        if (count > inputList.Count)
        {
            Debug.LogError("Count exceeds the number of available sprites. Returning all sprites.");
            count = inputList.Count; // Điều chỉnh count về số lượng sprite hiện có
        }

        // Danh sách để lưu sprite đã chọn
        List<Sprite> selectedSprites = new List<Sprite>();
        List<Sprite> tempList = new List<Sprite>(inputList); // Tạo một bản sao của inputList

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count); // Chọn một chỉ số ngẫu nhiên
            selectedSprites.Add(tempList[randomIndex]); // Thêm sprite vào danh sách đã chọn
            tempList.RemoveAt(randomIndex); // Loại bỏ sprite đã chọn khỏi danh sách tạm
        }

        return selectedSprites; // Trả về danh sách các sprite đã chọn
    }
    private void GenerateMatchItem(List<Sprite> list_Sprites)
    {
       

        if (itemUIPb == null || gridRoot == null) return;

        // lay chan so cap, vi du co 9 nhung phai lay chan de chia so cap
        // totalItem la tong so o, gia sư ta nhap 11 o thi no se le khong the chia ra cap, vi so cap la chan
        int totalItem = m_listLevelScriptsData[LevelSystemManager.Ins.CurrentLevel].nOPairs;
        int divItem = totalItem % 2;
        m_totalMatchItem = totalItem - divItem; // 9-1=8

        matchItems = new List<MatchItem>();
        list_Sprites = GetRandomSprites(list_Sprites, m_totalMatchItem);

        for (int i = 0; i < m_totalMatchItem; i++)
        {
            MatchItem matchItem = new MatchItem();
            matchItem.icon = list_Sprites[i];
            matchItem.Id = i;
            matchItems.Add(matchItem);
        }

        m_matchItemsCopy.AddRange(matchItems); // 1/2 tong so the dung trong game
        m_matchItemsCopy.AddRange(matchItems); 

        ShuffleMatchItems();
        ClearGrid();

        for (int i = 0;i < m_matchItemsCopy.Count; i++)
        {
            var matchItem = m_matchItemsCopy[i];

            var matchItemUIClone = Instantiate(itemUIPb,Vector3.zero,Quaternion.identity);

            RectTransform itemRectTransform = matchItemUIClone.GetComponent<RectTransform>();
            itemRectTransform.sizeDelta = new Vector2(100, 100); // Điều chỉnh kích thước theo ý muốn
            itemRectTransform.pivot = new Vector2(0.5f, 0.5f);

            matchItemUIClone.transform.SetParent(gridRoot);
            matchItemUIClone.transform.localScale = Vector3.one;
            matchItemUIClone.transform.localPosition = Vector3.zero;
            matchItemUIClone.UpdateFirstState(matchItem.icon);
            matchItemUIClone.Id = matchItem.Id;
            m_matchItemUIs.Add(matchItemUIClone);

            if (matchItemUIClone.btnComp)
            {
                matchItemUIClone.btnComp.onClick.RemoveAllListeners();
                matchItemUIClone.btnComp.onClick.AddListener(() =>
                {
                    if(m_isAnswerChecking) return;

                    m_answers.Add(matchItemUIClone);
                    matchItemUIClone.OpenAnimTrigger();
                    if(m_answers.Count == 2)
                    {
                        m_totalMoving++;
                        m_isAnswerChecking = true;
                        StartCoroutine(CheckAnswerCo());
                    }

                    matchItemUIClone.btnComp.enabled = false;
                });
            }
        }

        UpdateGrid(m_totalMatchItem);

        state = GameState.Playing;
        //if (DialogManager.Ins)
        //    GUIManager.Ins.ShowGameplay(true);
    }
    void UpdateGrid(int count)
    {
        
        switch (count)
        {
            case 2:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 2;
                gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

                // Đặt grid ở giữa màn hình 
                // Thiết lập anchorMin và anchorMax cho Middle Center
                gridRectTransform.anchorMin = new Vector2(0f, 0.5f); // Mỏ neo nhỏ nhất
                gridRectTransform.anchorMax = new Vector2(1f, 0.5f); // Mỏ neo lớn nhất
                
                // Đặt vị trí để căn giữa trong không gian của cha
                gridRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(247.5f, 202.5f);
                break;
            case 6:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 3;
                gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

                // Đặt grid ở giữa màn hình 
                // Thiết lập anchorMin và anchorMax cho Middle Center
                gridRectTransform.anchorMin = new Vector2(0f, 0.5f); // Mỏ neo nhỏ nhất
                gridRectTransform.anchorMax = new Vector2(1f, 0.5f); // Mỏ neo lớn nhất

                // Đặt vị trí để căn giữa trong không gian của cha
                gridRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(130f, 437.5f);
                break;
            default:
                break;
        }
    }

    private IEnumerator CheckAnswerCo()
    {
        bool isRight = m_answers[0] != null && m_answers[1] != null
            && m_answers[0].Id == m_answers[1].Id;
        yield return new WaitForSeconds(1f);

        if (m_answers != null && m_answers.Count == 2)
        {
            if (isRight)
            {
                m_rightMoving++;
                for (int i = 0; i < m_answers.Count; i++)
                {
                    var answer = m_answers[i];
                    if (answer)
                        answer.ExplodeAnimTrigger();
                    m_matchItemUIs.Remove(answer);
                    if (AudioController.Ins)
                        AudioController.Ins.PlaySound(AudioController.Ins.right);
                }    
            }
            else
            {
                for(int i = 0;i < m_answers.Count; i++)
                {
                    var answer = m_answers[i];
                    if(answer)
                        answer.OpenAnimTrigger();
                    if (AudioController.Ins)
                        AudioController.Ins.PlaySound(AudioController.Ins.wrong);
                }
            }
        }

        m_isAnswerChecking = false;
        m_answers.Clear();

        if(m_rightMoving == m_totalMatchItem)
        {
            Pref.bestMove = m_totalMoving;
            if (DialogManager.Ins)
                DialogManager.Ins.gameoverDialog.Show(true);
            if (AudioController.Ins)
                AudioController.Ins.PlaySound(AudioController.Ins.gameover);
            state = GameState.Completed;
            LevelSystemManager.Ins.LevelComplete(LevelSystemManager.Ins.CurrentLevel);
            LevelSystemManager.Ins.CurrentLevel++;
            if (LevelSystemManager.Ins.CurrentLevel > m_listLevelScriptsData.Count - 1)
            {
                LevelSystemManager.Ins.CurrentLevel = 0;
            }
            PlayerPrefs.SetInt("CurrentLevel", LevelSystemManager.Ins.CurrentLevel);
            Debug.Log("Level Completed!!!");
        }
    }

    private void ShuffleMatchItems()
    {
        if (m_matchItemsCopy == null || m_matchItemsCopy.Count == 0) return;

        for (int i = 0; i < m_matchItemsCopy.Count; i++)
        {
            {
                var temp = m_matchItemsCopy[i];
                if(temp != null)
                {
                    int randIdx = Random.Range(0, m_matchItemsCopy.Count);
                    m_matchItemsCopy [i] = m_matchItemsCopy[randIdx];
                    m_matchItemsCopy [randIdx] = temp;
                }
            }
        }
    }
    private void ClearGrid()
    {
        if (gridRoot == null) return;

        for(int i = 0; i < gridRoot.childCount; i++)
        {
            var child = gridRoot.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }
    }

}
