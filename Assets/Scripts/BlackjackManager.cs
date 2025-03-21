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
    public enum BlackjackState
    {
        Idle,
        Playing
    }
    public BlackjackState currentState;

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
    [Space(50)]
    [SerializeField] private List<Card> cards = new();
    [SerializeField] private List<Card> drawnCards = new();
    [SerializeField] private List<Card> dealerDrawnCards = new();

    [SerializeField] private int money = 5000; // Refer this to the actual money value in the script holding it
    private int cardValue = 0;
    private int dealerCardValue = 0;
    private int cardAceValue = 0;
    private int dealerCardAceValue = 0;
    private int currentBet = 0;

    private void CreateDeck()
    {
        cards.Clear();
        drawnCards.Clear();
        dealerDrawnCards.Clear();
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

    private void SetState(BlackjackState state)
    {
        if (state == currentState) return;
        currentState = state;
        switch (currentState)
        {
            case BlackjackState.Idle:
                {
                    betInput.interactable = true;
                    drawButton.interactable = true;
                    doubleButton.interactable = false;
                    hitButton.interactable = false;
                    standButton.interactable = false;
                    break;
                }
            case BlackjackState.Playing:
                {
                    betInput.interactable = false;
                    drawButton.interactable = false;
                    doubleButton.interactable = true;
                    hitButton.interactable = true;
                    standButton.interactable = true;
                    break;
                }
        }
    }

    private void CheckValidityOfDraw()
    {
        if (currentState != BlackjackState.Idle)
        {
            Debug.Log("Cannot draw while playing");
            return;
        }
        if (betInput.text == "")
        {
            Debug.Log("Must bet a value before drawing");
            return;
        }
        int betAmount = Convert.ToInt32(betInput.text);
        if (betAmount <= 0)
        {
            Debug.Log("Cannot bet 0 or a negative amount");
            return;
        }
        if (betAmount > money)
        {
            Debug.Log("Cannot bet more than you have");
            return;
        }
        InitialDraw();

    }

    private void InitialDraw()
    {
        CreateDeck();
        SetState(BlackjackState.Playing);
        currentBet = Convert.ToInt32(betInput.text);
        money -= currentBet;
        DrawPlayerCard();
        DrawPlayerCard();
        DrawDealerCard(false);
        DrawDealerCard(true);
        Debug.Log("yippee");
    }

    private void CheckValidityOfDouble()
    {
        if (currentState == BlackjackState.Idle)
        {
            Debug.Log("Can only double while playing");
            return;
        }
        if (currentBet > money)
        {
            Debug.Log("You do not have enough money to double");
            return;
        }
        Double();
    }

    private void Double()
    {
        money -= currentBet;
        currentBet *= 2;
        betInput.text = currentBet.ToString();
        CheckValidityOfHit();
        CheckValidityOfStand();
    }

    private void DrawPlayerCard()
    {
        //Check intoxication event
        Card card = cards[UnityEngine.Random.Range(0, cards.Count)];
        drawnCards.Add(card);
        cards.Remove(card);
        Instantiate(cardPrefab, cardHolder.transform);
        UpdatePlayerScore();
    }

    private void DrawDealerCard(bool hidden)
    {
        //Check intoxication event
    }

    private void CheckValidityOfHit()
    {
        if (currentState == BlackjackState.Idle)
        {
            Debug.Log("Can only hit while playing");
            return;
        }
        Hit();
    }

    private void Hit()
    {
        DrawPlayerCard();
    }

    private void CheckValidityOfStand()
    {
        if (currentState == BlackjackState.Idle)
        {
            Debug.Log("Can only stand while playing");
            return;
        }
        Stand();
    }

    private void Stand()
    {
        Debug.Log("stand");
    }

    private int CountAces(int initialValue, int aces)
    {
        int value = initialValue;
        int currentAces = aces;
        while (currentAces > 0 && value < 21)
        {
            currentAces--;
            value += 11;
        }
        if (value > 21 && currentAces > 0)
        {
            value -= 10;
            while (currentAces > 0)
            {
                currentAces--;
                value++;
            }
        }
        return value;
    }

    private void UpdatePlayerScore()
    {
        int aces = 0;
        int value = 0;
        for (int i = 0; i < drawnCards.Count; i++)
        {
            if (drawnCards[i] != null)
            {
                if (drawnCards[i].number >= 10)
                {
                    value += 10;
                }
                else if (drawnCards[i].number == 1)
                {
                    aces++;
                }
                else
                {
                    value += drawnCards[i].number;
                }
            }
        }
        value = CountAces(value, aces);
        cardValue = value;
        score.text = value.ToString();
        if (value > 21)
        {
            EndGame(false);
        }
    }

    private void UpdateDealerScore()
    {

    }

    private void EndGame(bool won)
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawButton.onClick.AddListener(CheckValidityOfDraw);
        doubleButton.onClick.AddListener(CheckValidityOfDouble);
        hitButton.onClick.AddListener(CheckValidityOfHit);
        standButton.onClick.AddListener(CheckValidityOfStand);

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