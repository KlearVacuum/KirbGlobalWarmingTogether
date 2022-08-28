using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScript : MonoBehaviour
{
    public int winCost;
    private int timesInvested;
    public int timesInvestedToWin;
    public Image _fillImage;
    public GameObject _finalBoi;

    public void BuyWin()
    {
        if (GlobalGameData.cash >= winCost)
        {
            GlobalGameData.cash -= winCost;
            timesInvested++;

            _fillImage.fillAmount = (float)timesInvested / timesInvestedToWin;
            Debug.Log(_fillImage.fillAmount);

            if (timesInvested >= timesInvestedToWin)
            {
                var go = Instantiate(_finalBoi);
                go.GetComponent<Animator>().CrossFade("kirbIG_xtra", 0, 0);
                LevelManager.Instance.NotifyWin();
            }
        }
        else
        {
            // put error action here
            Debug.Log("CANNOT AFFORD WIN THING");
        }
    }
}
