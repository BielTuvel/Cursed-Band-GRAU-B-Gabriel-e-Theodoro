using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public TextMeshProUGUI TextName;
    public TextMeshProUGUI TextAcc;
    public TextMeshProUGUI TextSolo;
    public TextMeshProUGUI TextPerf;
    public TextMeshProUGUI TextID;

    public Image ThisImage;

    public Card CardOnScreen;

    public Client Client;

    private void Start()
    {
        DisplayCard();
    }
    
    private void OnEnable()
    {
        Client.Deck.OnCardMove += DisplayCard;
    }

    private void OnDisable()
    {
        Client.Deck.OnCardMove -= DisplayCard;
    }

    private void DisplayCard()
    {
        if (Client.Deck.GetCardOnTop() != null)
        {
            CardOnScreen = Client.Deck.GetCardOnTop();
            TextName.text = CardOnScreen.Name;
            TextAcc.text = CardOnScreen.Accuracy.ToString();
            TextSolo.text = CardOnScreen.Solo.ToString();
            TextPerf.text = CardOnScreen.Performance.ToString();
            TextID.text = CardOnScreen.Id.ToString();
            ThisImage.sprite = CardOnScreen.CardSprite;
        }
    }
}
