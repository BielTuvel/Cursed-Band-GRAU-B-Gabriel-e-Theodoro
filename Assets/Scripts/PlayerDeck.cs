using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public Action OnCardMove;
    public List<Card> Deck = new List<Card>();
    
    private void Start()
    {
        
    }

    public Card GetCardOnTop()
    {
        if (Deck.Count > 0)
        {
            return Deck[0];
        }
        else
        {
            return null;
        }
    }

    public void RemoveCard()
    {
        Deck.Remove(Deck[0]);
        OnCardMove?.Invoke();
    }

    public void AddCard(Card card)
    {
        Deck.Add(card);
    }

    public void SendCardToBack()
    {
        Card TempCard = Deck[0];
        Deck.Remove(Deck[0]);
        Deck.Add(TempCard);
        OnCardMove?.Invoke();
    }
}
