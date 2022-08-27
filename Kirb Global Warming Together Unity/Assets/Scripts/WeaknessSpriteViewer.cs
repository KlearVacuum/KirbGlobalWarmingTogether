using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaknessSpriteViewer : MonoBehaviour
{
    public List<eTrashType> weaknessTypes;
    public List<WeaknessSprite> weaknessToSprites;
    public Sprite skullSprite;
    public List<Image> images;

    private int spriteNum;

    private void Start()
    {
        images[0].sprite = skullSprite;
        spriteNum = 0;
        if (weaknessTypes.Count > 0)
        {
            foreach (eTrashType trashType in weaknessTypes)
            {
                Sprite foundSprite = GetSpriteOfTrashType(trashType);
                if (foundSprite != null)
                {
                    spriteNum++;
                    images[spriteNum].gameObject.SetActive(true);
                    images[spriteNum].sprite = foundSprite;
                }
            }
        }
        while (spriteNum < images.Count - 1)
        {
            spriteNum++;
            images[spriteNum].gameObject.SetActive(false);
        }
    }

    private Sprite GetSpriteOfTrashType(eTrashType type)
    {
        foreach(var weakness in weaknessToSprites)
        {
            if (weakness.trashType == type)
            {
                return weakness.sprite;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct WeaknessSprite
{
    public eTrashType trashType;
    public Sprite sprite;
}
