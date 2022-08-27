using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    public int winCost;
    public int timesInvested;
    public int timesInvestedToWin;

    public void BuyWin()
    {
        if (GlobalGameData.cash >= winCost)
        {
            GlobalGameData.cash -= winCost;
            timesInvested++;
            if (timesInvested >= timesInvestedToWin)
            {
                // WIN
            }
        }
        else
        {
            // put error action here
            Debug.Log("CANNOT AFFORD WIN THING");
        }
    }
}
