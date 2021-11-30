using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 0)]
public class Card : ScriptableObject 
{
    public int Id;
    public string Name;

    public int Accuracy;
    public int Solo;
    public int Performance;

    public Sprite CardSprite;
}
