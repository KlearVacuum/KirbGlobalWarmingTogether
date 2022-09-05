using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public KeyCode panicKey;
    public KeyCode stopWorkKey;
    public float gameplayTime;
    public GameObject kirb;
    public int startingCash;
    public int kirbCost;
    public TextMeshProUGUI cashUI;
    private Animator cashUIAnimator;
    public TextMeshProUGUI timerUI;
    public TextMeshProUGUI populationUI;

    [HideInInspector]
    public int selectedKirbButton;

    public float panicTimer = 0.0f;
    public float panicCooldown = 0.0f;

    private void Awake()
    {
        _instance = this;

        GlobalGameData.ResetGameData();
        GlobalGameData.cash = startingCash;
        cashUI.text = "$" + GlobalGameData.cash;
        selectedKirbButton = 0;
        panicTimer = panicCooldown;

        cashUIAnimator = cashUI.gameObject.GetComponent<Animator>();
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

        cashUI.text = "$" + GlobalGameData.cash;
        if (LevelManager.Instance.GameState != GameState.Playing) { return; }

        if (panicTimer <= panicCooldown) {
            panicTimer += Time.deltaTime;
        } else {
            panicTimer = panicCooldown;
        }

        // temp panic key
        if (Input.GetKeyDown(panicKey))
        {
            Panic();
        }

        //if (Input.GetKeyDown(stopWorkKey))
        //{
        //    foreach (var kirb in GlobalGameData.allAiEntities)
        //    {
        //        kirb.StopCollectingTrash();
        //    }
        //}

        gameplayTime += Time.deltaTime;

        // time is in seconds
        int minutes = (int)(gameplayTime / 60f);
        int seconds = (int)(gameplayTime % 60f);
        string minuteText = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        string secondsText = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        timerUI.text = minuteText + ":" + secondsText;

        populationUI.text = GlobalGameData.allAiEntities.Count + " Alive";
    }

    public void Panic()
    {
        if (panicTimer < panicCooldown) 
        {
            return;
        }

        foreach (var kirb in GlobalGameData.allAiEntities)
        {
            kirb.panic = true;
        }

        panicTimer = 0;
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
        else
        {
            NoMoneyFeedback();
        }
    }

    public void NoMoneyFeedback()
    {
        // play no money error sound
        cashUIAnimator.Play("ErrorRedBlinkAnim");
    }

    public static void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
