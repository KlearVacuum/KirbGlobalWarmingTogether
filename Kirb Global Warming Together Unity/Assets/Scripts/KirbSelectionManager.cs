using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbSelectionManager : MonoBehaviour
{
    public List<KirbType> kirbTypes;
    public GameObject buttonPrefab;
    public GameObject winButton;
    public string winButtonText;

    GameObject buttonParent;
    TabGroupScript tabGroup;

    private void Awake()
    {
        buttonParent = gameObject;
        tabGroup = buttonParent.GetComponent<TabGroupScript>();
        foreach (KirbType kirb in kirbTypes)
        {
            GameObject button = Instantiate(buttonPrefab, buttonParent.transform);
            var buttonScript = button.GetComponent<TabButtonScript>();
            buttonScript.mTabGroup = tabGroup;
            buttonScript.mTabImageBackground.sprite = kirb.kirbSprite;
            button.GetComponentInChildren<WriteTextUI>().WriteText(kirb.name);
            button.GetComponent<KirbSpawnScript>().kirbType = kirb;
        }
        GameObject empty = new GameObject();
        GameObject emptySpace = Instantiate(empty, buttonParent.transform);
        emptySpace.AddComponent<RectTransform>();

        GameObject win = Instantiate(winButton, buttonParent.transform);
        win.GetComponent<TabButtonScript>().mTabGroup = tabGroup;
        win.GetComponentInChildren<WriteTextUI>().WriteText(winButtonText);

        gameObject.transform.GetChild(0).GetComponent<KirbSpawnScript>().SetKirbType();
    }
}

[System.Serializable]
public struct KirbType
{
    public string name;
    public GameObject kirb;
    public int cost;
    public Sprite kirbSprite;
}
