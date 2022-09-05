using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kirb Type")]
public class KirbType : ScriptableObject
{
    public new string name;
    public GameObject kirb;
    public int cost;
    public Sprite kirbSprite;
    public Color menuColor;
}