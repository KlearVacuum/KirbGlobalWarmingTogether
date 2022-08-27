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

    public List<WeaknessSprite> weaknessSprites; 

    private void Awake()
    {
        buttonParent = gameObject;
        tabGroup = buttonParent.GetComponent<TabGroupScript>();
        for (int i = 0; i < 6; ++i)
        {
            if (i < kirbTypes.Count)
            {
                GameObject button = Instantiate(buttonPrefab, buttonParent.transform);
                var buttonScript = button.GetComponent<TabButtonScript>();
                buttonScript.mTabGroup = tabGroup;
                buttonScript.mTabImageBackground.sprite = kirbTypes[i].kirbSprite;
                button.GetComponentInChildren<WriteTextUI>().WriteText(kirbTypes[i].name + "\n" + "$" + kirbTypes[i].cost);

                AIEntity kirb = kirbTypes[i].kirb.GetComponent<AIEntity>();
                var viewer = button.GetComponentInChildren<WeaknessSpriteViewer>();

                if (kirb.trashTypeWeakness != null && kirb.trashTypeWeakness.Count > 0)
                {
                    foreach (var weakness in kirb.trashTypeWeakness)
                    {
                        viewer.weaknessTypes.Add(weakness);
                    }
                }

                button.GetComponent<KirbSpawnScript>().kirbType = kirbTypes[i];
            }
            else
            {
                GameObject empty = new GameObject();
                GameObject emptySpace = Instantiate(empty, buttonParent.transform);
                emptySpace.AddComponent<RectTransform>();
            }
        }

        GameObject win = Instantiate(winButton, buttonParent.transform);
        win.GetComponent<TabButtonScript>().mTabGroup = tabGroup;
        win.GetComponentInChildren<WriteTextUI>().WriteText(winButtonText + "\n" + "$" + win.GetComponent<WinScript>().winCost);

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
