using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KirbSpawnScript : MonoBehaviour
{
    public KirbType kirbType;
    public int selectedKirbButtonNum;
    public TextMeshProUGUI toolTip;

    bool toolTipSet;

    private void Start()
    {
        toolTipSet = false;
    }
    private void Update()
    {
        if (!toolTipSet && kirbType.kirb != null)
        {
            toolTip.text = kirbType.name + "\n$" + kirbType.cost + "\n" +
                           "Allergic to: ";
            AIEntity kirb = kirbType.kirb.GetComponent<AIEntity>();
            if (kirb.trashTypeWeakness == null || kirb.trashTypeWeakness.Count == 0)
            {
                toolTip.text += "Nothing!";
            }
            else
            {
                foreach (var weakness in kirb.trashTypeWeakness)
                {
                    switch (weakness)
                    {
                        case eTrashType.Plastic:
                            toolTip.text += "<color=#54FB64>Plastics" + @"</color>";
                            break;
                        case eTrashType.Glass:
                            toolTip.text += "<color=#56E1FF>Glass" + @"</color>";
                            break;
                        case eTrashType.Metal:
                            toolTip.text += "<color=#000000>Metals" + @"</color>";
                            break;
                        case eTrashType.General:
                            toolTip.text += "<color=#B66200>Organics" + @"</color>";
                            break;

                    }
                    if (weakness != kirb.trashTypeWeakness[kirb.trashTypeWeakness.Count - 1])
                    {
                        toolTip.text += ", ";
                    }
                }
            }
            toolTip.text += "\n" + kirb.description;
            toolTipSet = true;
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
