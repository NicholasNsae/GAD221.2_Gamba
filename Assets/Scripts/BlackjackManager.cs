using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BlackjackManager : MonoBehaviour
{
    public enum Suit
    {
        Spades,
        Clubs,
        Hearts,
        Diamonds
    }

    public UnityEvent BetMade = new UnityEvent();
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Image[] imageList;
    [SerializeField] private int deckNumber;
    [SerializeField] private GameObject cardHolder;
    [SerializeField] private GameObject dealerCardHolder;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI dealerScore;

    [SerializeField] private List<Card> cards = new();
    [SerializeField] private List<Card> drawnCards = new();
    [SerializeField] private List<Card> dealerDrawnCards = new();

    private int cardValue = 0;

    private void CreateDeck()
    {
        cards.Clear();
        for (int deck = 0; deck < deckNumber; deck++)
        {
            for (int suit = 0; suit < 4; suit++)
            {
                for (int num = 1; num < 14; num++)
                {
                    cards.Add(new Card
                    {
                        suit = (Suit)suit,
                        number = num
                    });
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class Card
{
    public BlackjackManager.Suit suit;
    public int number;
    public GameObject obj;
}