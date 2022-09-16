using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KirbSpawnScript : MonoBehaviour
{
    public KirbType kirbType;
    public int selectedKirbButtonNum;

    public GameObject TooltipPrefab;
    GameObject myTooltip;

    [HideInInspector]
    public TextMeshProUGUI toolTipText;

    bool toolTipSet;

    private void Awake()
    {
        myTooltip = Instantiate(TooltipPrefab, GameManager._instance.toolTipParent.transform);
        TabButtonScript tabButton = GetComponent<TabButtonScript>();
        if (tabButton != null)
        {
            tabButton.toolTip = myTooltip;
        }
        toolTipText = myTooltip.GetComponentInChildren<TextMeshProUGUI>();
        toolTipSet = false;
    }
    private void Update()
    {
        if (!toolTipSet && kirbType.kirb != null)
        {
            toolTipText.text = kirbType.name + "\n$" + kirbType.cost + "\n" +
                           "Allergic to: ";
            AIEntity kirb = kirbType.kirb.GetComponent<AIEntity>();
            if (kirb.trashTypeWeakness == null || kirb.trashTypeWeakness.Count == 0)
            {
                toolTipText.text += "Nothing!";
            }
            else
            {
                foreach (var weakness in kirb.trashTypeWeakness)
                {
                    switch (weakness)
                    {
                        case eTrashType.Plastic:
                            toolTipText.text += "<color=#56E1FF>Plastics" + @"</color>";
                            break;
                        case eTrashType.Glass:
                            toolTipText.text += "<color=#6AE066>Glass" + @"</color>";
                            break;
                        case eTrashType.Metal:
                            toolTipText.text += "<color=#BEBEBE>Metals" + @"</color>";
                            break;
                        case eTrashType.General:
                            toolTipText.text += "<color=#B66200>Organics" + @"</color>";
                            break;

                    }
                    if (weakness != kirb.trashTypeWeakness[kirb.trashTypeWeakness.Count - 1])
                    {
                        toolTipText.text += ", ";
                    }
                }
            }
            toolTipText.text += "\n" + kirb.description;
            toolTipText.text += "\n\n" + "Crowned: " + kirb.crownDescription;
            toolTipSet = true;
            myTooltip.SetActive(false);
        }
    }

    public void SetKirbType()
    {
        GameManager._instance.kirb = kirbType.kirb;
        GameManager._instance.kirbCost = kirbType.cost;
        // GameManager._instance.kirbName = kirbType.name;
        GameManager._instance.selectedKirbButton = selectedKirbButtonNum;
    }
}
