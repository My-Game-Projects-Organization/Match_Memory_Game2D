﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleGameManager : MonoBehaviour
{
    public MatchItem[] matchItems;
    public MatchItemUI itemUIPb;
    public Transform gridRoot;
    private List<MatchItem> m_matchItemsCopy;
    private List<MatchItemUI> m_matchItemUIs;
    private List<MatchItemUI> m_answers;
    private float m_timeCounting;
    private int m_totalMatchItem;
    private int m_totalMoving;
    private int m_rightMoving;
    private bool m_isAnswerChecking;


    public int TotalMoving { get => m_totalMoving; }
    public int RightMoving { get => m_rightMoving; }

    private void Awake()
    {
        m_matchItemsCopy = new List<MatchItem>();
        m_matchItemUIs = new List<MatchItemUI>();
        m_answers = new List<MatchItemUI>();
        PlayGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(MatchItemUI matchItemUI in m_matchItemUIs)
            {
                //Debug.Log(matchItemUI.is)
            }
        }
    }

    public void PlayGame()
    {
        GenerateMatchItem();
    }

    private void GenerateMatchItem()
    {
        if (matchItems == null || matchItems.Length <= 0 || itemUIPb == null || gridRoot == null) return;

        // lay chan so cap, vi du co 9 nhung phai lay chan de chia so cap
        // totalItem la tong so o, gia sư ta nhap 11 o thi no se le khong the chia ra cap, vi so cap la chan
        int totalItem = matchItems.Length;
        int divItem = totalItem % 2;
        m_totalMatchItem = totalItem - divItem; // 9-1=8

        for (int i = 0; i < m_totalMatchItem; i++)
        {
            var matchItem = matchItems[i];
            if (matchItem != null)
                matchItem.Id = i;
        }

        m_matchItemsCopy.AddRange(matchItems); // 1/2 tong so the dung trong game
        m_matchItemsCopy.AddRange(matchItems);

        ShuffleMatchItems();
        ClearGrid();

        for (int i = 0; i < m_matchItemsCopy.Count; i++)
        {
            var matchItem = m_matchItemsCopy[i];

            var matchItemUIClone = Instantiate(itemUIPb, Vector3.zero, Quaternion.identity);
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
                    if (m_isAnswerChecking) return;

                    m_answers.Add(matchItemUIClone);
                    matchItemUIClone.OpenAnimTrigger();
                    if (m_answers.Count == 2)
                    {
                        m_totalMoving++;
                        m_isAnswerChecking = true;
                        StartCoroutine(CheckAnswerCo());
                    }

                    matchItemUIClone.btnComp.enabled = false;
                });
            }
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
                    if (AudioController.Ins)
                        AudioController.Ins.PlaySound(AudioController.Ins.right);
                }
            }
            else
            {
                for (int i = 0; i < m_answers.Count; i++)
                {
                    var answer = m_answers[i];
                    if (answer)
                        answer.OpenAnimTrigger();
                    if (AudioController.Ins)
                        AudioController.Ins.PlaySound(AudioController.Ins.wrong);
                }
            }
        }

        m_isAnswerChecking = false;
        m_answers.Clear();

        if (m_rightMoving == m_totalMatchItem)
        {
            Pref.bestMove = m_totalMoving;
            if (GUIManager.Ins)
                GUIManager.Ins.gameoverDialog.Show(true);
            if (AudioController.Ins)
                AudioController.Ins.PlaySound(AudioController.Ins.gameover);
            Debug.Log("Gameover!!!");
        }
    }

    private void ShuffleMatchItems()
    {
        if (m_matchItemsCopy == null || m_matchItemsCopy.Count == 0) return;

        for (int i = 0; i < m_matchItemsCopy.Count; i++)
        {
            {
                var temp = m_matchItemsCopy[i];
                if (temp != null)
                {
                    int randIdx = Random.Range(0, m_matchItemsCopy.Count);
                    m_matchItemsCopy[i] = m_matchItemsCopy[randIdx];
                    m_matchItemsCopy[randIdx] = temp;
                }
            }
        }
    }
    private void ClearGrid()
    {
        if (gridRoot == null) return;

        for (int i = 0; i < gridRoot.childCount; i++)
        {
            var child = gridRoot.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }
    }
}
