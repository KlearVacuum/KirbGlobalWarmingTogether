using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButtonScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroupScript mTabGroup;
    public Image mTabImageBackground;

    public void OnPointerClick(PointerEventData eventData)
    {
        mTabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mTabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mTabGroup.OnTabExit(this);
    }

    private void Start()
    {
        mTabImageBackground = GetComponent<Image>();
        mTabGroup.SubscribeButton(this);
        if (mTabGroup.allSameColor)
        {
            mTabImageBackground.color = mTabGroup.mTabIdleColor;
        }
        else
        {
            mTabImageBackground.color = mTabImageBackground.color * new Vector4(1, 1, 1, mTabGroup.mStartingOpacity);
        }
    }
}