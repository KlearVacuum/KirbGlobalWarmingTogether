using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanicButton : MonoBehaviour
{
    [SerializeField] private Button panicButton = null;
    [SerializeField] private Image image = null;
    [SerializeField] private GameManager gameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.panicTimer >= gameManager.panicCooldown) 
        {
            panicButton.interactable = true;
        } else {
            panicButton.interactable = false;
            image.fillAmount = gameManager.panicTimer / gameManager.panicCooldown;
        }
    }
}
