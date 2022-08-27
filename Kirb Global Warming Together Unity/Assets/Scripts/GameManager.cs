using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public float gameplayTime;
    public GameObject kirb;
    public int startingCash;
    public int kirbCost;
    public TextMeshProUGUI cashUI;
    public TextMeshProUGUI timerUI;
    public TextMeshProUGUI populationUI;

    [HideInInspector]
    public int selectedKirbButton;

    private void Awake()
    {
        _instance = this;

        GlobalGameData.ResetGameData();
        GlobalGameData.cash = startingCash;
        cashUI.text = "$" + GlobalGameData.cash;
        selectedKirbButton = 0;
    }

    private void Update()
    {
        /* Moved to SpawnKirb() */
        // if (Input.GetMouseButtonDown(0) && GlobalGameData.cash >= kirbCost)
        // {
        //     GlobalGameData.cash -= kirbCost;
        //     Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     worldPos.z = 0;
        //     Instantiate(kirb, worldPos, Quaternion.identity);
        // }

        // temp panic key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (var kirb in GlobalGameData.allAiEntities)
            {
                kirb.panic = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (var kirb in GlobalGameData.allAiEntities)
            {
                kirb.collectTrash = false;
            }
        }

        cashUI.text = "$" + GlobalGameData.cash;
        gameplayTime += Time.deltaTime;

        // time is in seconds
        int minutes = (int)(gameplayTime / 60f);
        int seconds = (int)gameplayTime - minutes;
        string minuteText = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        string secondsText = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        timerUI.text = minuteText + ":" + secondsText;

        populationUI.text = GlobalGameData.allAiEntities.Count + " Alive";
    }

    public void SpawnKirb()
    {
        if (GlobalGameData.cash >= kirbCost)
        {
            GlobalGameData.cash -= kirbCost;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Instantiate(kirb, worldPos, Quaternion.identity);
        }
    }
}
