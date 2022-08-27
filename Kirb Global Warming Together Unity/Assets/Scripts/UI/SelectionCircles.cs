using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionCircles : MonoBehaviour
{
    [SerializeField]
    private int selectedCircle;
    public List<Image> myCircles;

    private void Start()
    {
        foreach (var circle in myCircles)
        {
            circle.enabled = false;
        }
        myCircles[selectedCircle].enabled = true;
    }

    void Update()
    {
        if (selectedCircle != GameManager._instance.selectedKirbButton)
        {
            myCircles[selectedCircle].enabled = false;
            selectedCircle = GameManager._instance.selectedKirbButton;
            myCircles[selectedCircle].enabled = true;
        }
    }
}
