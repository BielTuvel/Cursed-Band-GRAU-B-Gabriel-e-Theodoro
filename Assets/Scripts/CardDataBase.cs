using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public List<Card> CardList = new List<Card>();
    public List<int> UsedIndices = new List<int>();

    private int _cardIndex;

    public void Start()
    {
        UsedIndices.Add(10);
    }

    public Card GetCard()
    {
        _cardIndex = Random.Range(0, CardList.Count);

        while(UsedIndices.Contains(_cardIndex))
        {
            _cardIndex = Random.Range(0, CardList.Count);
        }

        Card card = CardList[_cardIndex];
        UsedIndices.Add(_cardIndex);

        //CardList.Remove(CardList[index]);

        return card;
    }
}
