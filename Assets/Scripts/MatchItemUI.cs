using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchItemUI : MonoBehaviour
{
    private int m_id;
    public Sprite bg;
    public Sprite backBG;
    public Image itemBG;
    public Image itemIcon;
    public Button btnComp;
    private bool m_isOpened;
    private Animator m_anim;

    public int Id { get => m_id; set => m_id = value; }
    public bool IsOpened { get => m_isOpened; set => m_isOpened = value; }

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    public void UpdateFirstState(Sprite icon)
    {
        if (itemBG)
        {
            itemBG.sprite = backBG;
        }
        if (itemIcon)
        {
            itemIcon.sprite = icon;
            itemIcon.gameObject.SetActive(false);
        }
    }
    
    public void ChangeState()
    {
        IsOpened = !IsOpened;

        if(itemBG)
            itemBG.sprite = IsOpened ? bg : backBG;
        if (itemIcon)
        {
            itemIcon.gameObject.SetActive(IsOpened);
        }
    }

    public void OpenAnimTrigger()
    {
        if (m_anim)
        {
            m_anim.SetBool(AnimState.Flip.ToString(), true);
        }
    }

    public void ExplodeAnimTrigger()
    {
        if (m_anim)
        {
            m_anim.SetBool(AnimState.Explode.ToString(), true);
        }
    }
    public void BackToIdle()
    {
        if (m_anim)
        {
            m_anim.SetBool(AnimState.Flip.ToString(), false);
        }
        if(btnComp)
            btnComp.enabled = !IsOpened;
    }
}
