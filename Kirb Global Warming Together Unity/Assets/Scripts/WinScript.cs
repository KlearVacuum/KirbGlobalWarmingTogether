using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinScript : MonoBehaviour
{
    public int winCost;
    [Tooltip("Every purchase increases winCost by this amount")]
    public float winMult;
    private int timesInvested;
    public int timesInvestedToWin;
    public Image _fillImage;
    public GameObject _finalBoi;
    public string winButtonText;
    private WriteTextUI winCostText;
    
    public TextMeshProUGUI toolTip;
    bool toolTipSet;

    private AudioSource _aSource;
    public AudioClip _aclipKaChing;

    private void Awake()
    {
        _aSource = GetComponent<AudioSource>();
        winCostText = GetComponentInChildren<WriteTextUI>();
    }

    private void Start()
    {
        winCostText.WriteText(winButtonText + "\n" + "$" + winCost);
        toolTipSet = false;
    }

    private void Update()
    {
        if (!toolTipSet)
        {
            toolTip.text = winButtonText + "\n" + "$" + winCost;
            toolTipSet = true;
        }
    }

    public void BuyWin()
    {
        if (GlobalGameData.cash >= winCost)
        {
            GlobalGameData.cash -= winCost;
            timesInvested++;
            winCost = (int)((float)winCost * winMult);
            _aSource.PlayOneShot(_aclipKaChing);

            _fillImage.fillAmount = (float)timesInvested / timesInvestedToWin;
            // Debug.Log(_fillImage.fillAmount);

            if (timesInvested >= timesInvestedToWin)
            {
                // no more cost
                winCostText.WriteText(winButtonText);
                toolTip.text = winButtonText;
                var go = Instantiate(_finalBoi);
                go.GetComponent<Animator>().CrossFade("kirbIG_xtra", 0, 0);
                LevelManager.Instance.NotifyWin();
            }
            else
            {
                // update new cost
                winCostText.WriteText(winButtonText + "\n" + "$" + winCost);
                toolTipSet = false;
            }
        }
        else
        {
            // put error action here
            GameManager._instance.NoMoneyFeedback();
            Debug.Log("CANNOT AFFORD WIN THING");
        }
    }
}
