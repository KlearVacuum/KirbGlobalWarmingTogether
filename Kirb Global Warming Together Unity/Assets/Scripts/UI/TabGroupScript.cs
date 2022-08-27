using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupScript : MonoBehaviour
{
    public List<TabButtonScript> mTabButtonsList;
    public Color mTabIdleColor;
    public Color mTabHoverColor;
    public Color mTabSelectedColor;

    // public GameObject[] TabButtonPopups;

    TabButtonScript mCurrentHoveredTab = null;
    AudioSource mSoundSource;

    private void Awake()
    {
        mSoundSource = GetComponent<AudioSource>();
    }

    public void SubscribeButton(TabButtonScript button)
    {
        if (mTabButtonsList == null)
        {
            mTabButtonsList = new List<TabButtonScript>();
        }
        mTabButtonsList.Add(button);
    }

    public void OnTabEnter(TabButtonScript button)
    {
        ResetTabs();
        mCurrentHoveredTab = button;
        button.mTabImageBackground.color = mTabHoverColor;

        // Show pop-up of hovered button
        //int index = button.transform.GetSiblingIndex();
        //for (int i = 0; i < TabButtonPopups.Length; ++i)
        //{
        //    if (i == index) TabButtonPopups[i].SetActive(true);
        //    else TabButtonPopups[i].SetActive(false);
        //}
    }

    public void OnTabExit(TabButtonScript button)
    {
        // Hide pop-up of currently hovered button
        //int index = button.transform.GetSiblingIndex();
        //TabButtonPopups[index].SetActive(false);

        ResetTabs();
    }

    public void OnTabSelected(TabButtonScript button)
    {
        ResetTabs();
        mCurrentHoveredTab = button;
        StartCoroutine(SetSelectedColorCoroutine(button));
    }

    public void ResetTabs()
    {
        // Set all button colours to idle
        foreach (TabButtonScript button in mTabButtonsList)
        {
            button.mTabImageBackground.color = mTabIdleColor;
        }
        mCurrentHoveredTab = null;
    }

    IEnumerator SetSelectedColorCoroutine(TabButtonScript button)
    {
        button.mTabImageBackground.color = mTabSelectedColor;
        yield return new WaitForSeconds(0.05f);
        if (mCurrentHoveredTab != null && mCurrentHoveredTab == button)
        {
            button.mTabImageBackground.color = mTabHoverColor;
        }
        else
        {
            button.mTabImageBackground.color = mTabIdleColor;
        }
    }
}
