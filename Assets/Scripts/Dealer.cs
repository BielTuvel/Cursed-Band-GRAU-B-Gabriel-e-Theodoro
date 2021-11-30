using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public PlayerDeck[] PlayerDecks;
    public CardDataBase DataBase;

    public int DeckSize;

    private void Awake()
    {
        CreateDecks();
    }

    public void CreateDecks() 
    {
        for (int i = 0; i < DeckSize; i++)
        {
            PlayerDecks[0].Deck[i] = DataBase.GetCard();
            PlayerDecks[1].Deck[i] = DataBase.GetCard();
        }
    }
}
