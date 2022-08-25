using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject kirb;
    public int kirbCost;
    public TextMeshProUGUI cashUI;

    private void Awake()
    {
        GlobalGameData.ResetGameData();
        cashUI.text = "$" + GlobalGameData.cash;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GlobalGameData.cash >= kirbCost)
        {
            GlobalGameData.cash -= kirbCost;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Instantiate(kirb, worldPos, Quaternion.identity);
        }

        // temp panic key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (var kirb in GlobalGameData.allAiEntities)
            {
                kirb.panic = true;
            }
        }

        cashUI.text = "$" + GlobalGameData.cash;
    }
}
