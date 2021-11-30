using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string AtributeType;

    public PlayerDeck[] Decks;
    public Server Server;
    public Dealer Dealer;

    private int _clientIndex = 0;
    private bool _hasGameStarted;

    private void OnEnable()
    {
        Server.OnPlayersConnected += StartGame;
    }

    private void OnDisable()
    {
        Server.OnPlayersConnected -= StartGame;
    }

    private void Update()
    {
        if(_hasGameStarted)
        {

        }
    }

    public void WhatType (string atribute)
    {
        AtributeType = atribute;
        Debug.Log("Atributo Escolhido: " + AtributeType);
    }

    public void Desafiar()
    {
        if (AtributeType != "")
        {
            if (AtributeType == "Accuracy")
            {
                if (Decks[0].GetCardOnTop().Accuracy > Decks[1].GetCardOnTop().Accuracy)
                {
                    Debug.Log("Player 1 VENCE!");
                    Decks[0].AddCard(Decks[1].GetCardOnTop());
                    Decks[1].RemoveCard();
                    Decks[0].SendCardToBack();
                }
                else
                {
                    Debug.Log("Player 2 VENCE!");
                    Decks[1].AddCard(Decks[0].GetCardOnTop());
                    Decks[0].RemoveCard();
                    Decks[1].SendCardToBack();
                }
            }

            if (AtributeType == "Solo")
            {
                if (Decks[0].GetCardOnTop().Solo > Decks[1].GetCardOnTop().Solo)
                {
                    Debug.Log("Player 1 VENCE!");
                    Decks[0].AddCard(Decks[1].GetCardOnTop());
                    Decks[1].RemoveCard();
                    Decks[0].SendCardToBack();
                }
                else
                {
                    Debug.Log("Player 2 VENCE!");
                    Decks[1].AddCard(Decks[0].GetCardOnTop());
                    Decks[0].RemoveCard();
                    Decks[1].SendCardToBack();
                }
            }

            if (AtributeType == "Performance")
            {
                if (Decks[0].GetCardOnTop().Performance > Decks[1].GetCardOnTop().Performance)
                {
                    Debug.Log("Player 1 VENCE!");
                    Decks[0].AddCard(Decks[1].GetCardOnTop());
                    Decks[1].RemoveCard();
                    Decks[0].SendCardToBack();
                }
                else
                {
                    Debug.Log("Player 2 VENCE!");
                    Decks[1].AddCard(Decks[0].GetCardOnTop());
                    Decks[0].RemoveCard();
                    Decks[1].SendCardToBack();
                }
            }
        }
    }

    private void StartGame()
    {
        //Desabilita painel

        _hasGameStarted = true;
    }
}
