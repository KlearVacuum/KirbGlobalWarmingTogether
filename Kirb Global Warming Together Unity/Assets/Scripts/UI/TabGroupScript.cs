using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupScript : MonoBehaviour
{
    public List<TabButtonScript> mTabButtonsList;
    public Color mTabIdleColor;
    public Color mTabHoverColor;
    public Color mTabSelectedColor;
    public bool allSameColor = true;

    private Dictionary<TabButtonScript, Color> tabToStartingColor;
    [Range(0,1)]
    public float mStartingOpacity;
    [Range(0, 1)]
    public float mOpacityOnHover;

    // public GameObject[] TabButtonPopups;

    TabButtonScript mCurrentHoveredTab = null;
    AudioSource mSoundSource;

    private void Awake()
    {
        mSoundSource = GetComponent<AudioSource>();
        tabToStartingColor = new Dictionary<TabButtonScript, Color>();
    }

    public void SubscribeButton(TabButtonScript button)
    {
        if (mTabButtonsList == null)
        {
            mTabButtonsList = new List<TabButtonScript>();
        }
        mTabButtonsList.Add(button);
        tabToStartingColor.Add(button, button.mTabImageBackground.color);
    }

    public void OnTabEnter(TabButtonScript button)
    {
        ResetTabs();
        mCurrentHoveredTab = button;

        KeyValuePair<TabButtonScript, Color> foundMap = default(KeyValuePair<TabButtonScript, Color>);

        foreach (var map in tabToStartingColor)
        {
            if (button == map.Key)
            {
                foundMap = map;
                continue;
            }
        }
        if (allSameColor)
        button.mTabImageBackground.color = mTabHoverColor;
        else
        {
            foundMap.Key.mTabImageBackground.color = foundMap.Value * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, mOpacityOnHover);
        }

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
        if (allSameColor)
        {
            foreach (TabButtonScript button in mTabButtonsList)
            {
                button.mTabImageBackground.color = mTabIdleColor;

            }
        }
        else
        {
            foreach (TabButtonScript button in mTabButtonsList)
            {
                foreach (var map in tabToStartingColor)
                {
                    if (button == map.Key)
                    {
                        button.mTabImageBackground.color = map.Value * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, mStartingOpacity);
                        continue;
                    }
                }
            }
        }
        mCurrentHoveredTab = null;
    }

    IEnumerator SetSelectedColorCoroutine(TabButtonScript button)
    {
        KeyValuePair<TabButtonScript, Color> foundMap = default(KeyValuePair<TabButtonScript, Color>);

        foreach (var map in tabToStartingColor)
        {
            if (button == map.Key)
            {
                foundMap = map;
                continue;
            }
        }

        if (allSameColor)
        button.mTabImageBackground.color = mTabSelectedColor;
        else
        {
            foundMap.Key.mTabImageBackground.color = foundMap.Value * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, mOpacityOnHover);
        }

        yield return new WaitForSeconds(0.05f);

        if (mCurrentHoveredTab != null && mCurrentHoveredTab == button)
        {
            if (allSameColor) button.mTabImageBackground.color = mTabHoverColor;
            else
            {
                foundMap.Key.mTabImageBackground.color = foundMap.Value * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, mOpacityOnHover);
            }
        }
        else
        {
            if (allSameColor)
            {
                button.mTabImageBackground.color = mTabIdleColor;
            }
            else
            {
                foundMap.Key.mTabImageBackground.color = foundMap.Value * new Vector4(1, 1, 1, 0) + new Color(0, 0, 0, mStartingOpacity);
            }
        }
    }
}
