using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackjackManager : MonoBehaviour
{
    public enum Suit
    {
        Spades,
        Clubs,
        Hearts,
        Diamonds
    }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Image[] imageList;
    [SerializeField] private int deckNumber;
    [Header("Events")]
    public UnityEvent BetMade = new UnityEvent();
    [Header("UI Elements")]
    [SerializeField] private GameObject cardHolder;
    [SerializeField] private GameObject dealerCardHolder;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI dealerScore;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private TMP_InputField betInput;
    [Header("Cards")]
    [SerializeField] private List<Card> cards = new();
    [SerializeField] private List<Card> drawnCards = new();
    [SerializeField] private List<Card> dealerDrawnCards = new();

    private int cardValue = 0;
    private int dealerCardValue = 0;
    private int currentBet = 0;
    private bool ableToHit = true;

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
                        number = num,
                        hidden = false
                    });
                }
            }
        }
    }

    private void InitialDraw()
    {

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
    public bool hidden;
    public GameObject obj;
}