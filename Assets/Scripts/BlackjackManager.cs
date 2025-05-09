using System;
using System.Collections.Generic;
using System.Collections;
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
        InitialDraw,
        Playing
    }
    public BlackjackState currentState;

    public enum EndState
    {
        Win,
        Draw,
        Loss,
        NaturalWin
    }

    [SerializeField] private GameObject cardPrefab;
    //[SerializeField] private Image[] imageList;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] spadeCards = new Sprite[13];
    [SerializeField] private Sprite[] clubCards = new Sprite[13];
    [SerializeField] private Sprite[] heartCards = new Sprite[13];
    [SerializeField] private Sprite[] diamondCards = new Sprite[13];
    [SerializeField] private Texture[] chipParticles;
    [SerializeField] private int deckNumber;
    [SerializeField] private int minimumBet;
    public int MinimumBet => minimumBet;
    [Header("Events")]
    public UnityEvent EndOfHand = new UnityEvent();
    public event Action<EndState, int> HandEnded; // int = how much money was won/lost
    public event Action<int> HandStarted; // int = how much the player bet
    public event Action<int> DoubledDown; // int = player's total bet after doubling down
    [Header("UI Elements")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cardHolder;
    [SerializeField] private GameObject dealerCardHolder;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI dealerScore;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI dealerFeedbackText;
    [SerializeField] private TextMeshProUGUI statusFieldText;
    [Space(50)]
    [SerializeField] private List<Card> cards = new();
    [SerializeField] private List<Card> drawnCards = new();
    [SerializeField] private List<Card> dealerDrawnCards = new();
    [SerializeField] private List<GameObject> displayCards = new();
    [SerializeField] private Color standardTextColour = Color.white;
    [SerializeField] private Color winTextColour = Color.green;
    [SerializeField] private Color loseTextColour = Color.red;
    [SerializeField] private Color drawTextColour = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color naturalWinTextColour = Color.yellow;
    

    [SerializeField] private ParticleManager particleManager;
    [SerializeField] private PhoneManager phoneManager;

    private int cardValue = 0;
    private int dealerCardValue = 0;
    private int cardAceValue = 0;
    private int dealerCardAceValue = 0;
    private int currentBet = 0;
    private bool drawn;

    private void CreateDeck()
    {
        foreach (GameObject card in displayCards) Destroy(card);
        cards.Clear();
        drawnCards.Clear();
        dealerDrawnCards.Clear();
        displayCards.Clear();
        for (int deck = 0; deck < deckNumber; deck++)
        {
            for (int suit = 0; suit < 4; suit++)
            {
                for (int num = 1; num < 14; num++)
                {
                    Sprite cardSprite = null;
                    switch ((Suit) suit)
                    {
                        case Suit.Spades:
                            cardSprite = spadeCards[num - 1];
                            break;
                        case Suit.Clubs:
                            cardSprite = clubCards[num - 1];
                            break;
                        case Suit.Hearts:
                            cardSprite = heartCards[num - 1];
                            break;
                        case Suit.Diamonds:
                            cardSprite = diamondCards[num - 1];
                            break;
                    }

                    cards.Add(new Card
                    {
                        suit = (Suit)suit,
                        number = num,
                        hidden = false,
                        sprite = cardSprite
                    });
                }
            }
        }
    }

    private void CreateDisplayCard(Card card, bool playerSide)
    {
        GameObject physicalCard;
        if (playerSide) physicalCard = Instantiate(cardPrefab, cardHolder.transform);
        else physicalCard = Instantiate(cardPrefab, dealerCardHolder.transform);
        if (card.hidden)
        {
            physicalCard.GetComponent<Image>().sprite = cardBack;
        }
        else
        {
            physicalCard.GetComponent<Image>().sprite = card.sprite;
        }
        displayCards.Add(physicalCard);
        card.obj = physicalCard;
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
                    doubleButton.interactable = !drawn;
                    hitButton.interactable = true;
                    standButton.interactable = true;
                    score.color = standardTextColour;
                    dealerScore.color = standardTextColour;
                    feedbackText.gameObject.SetActive(false);
                    dealerFeedbackText.gameObject.SetActive(false);
                    break;
                }
        }
    }

    public void ResetState()
    {
        currentState = BlackjackState.Idle;
        score.color = standardTextColour;
        dealerScore.color = standardTextColour;
        feedbackText.gameObject.SetActive(false);
        dealerFeedbackText.gameObject.SetActive(false);
        betInput.text = string.Empty;
        foreach (GameObject card in displayCards) Destroy(card);
        score.text = "0";
        dealerScore.text = "0";
        phoneManager.BankValue = phoneManager.StartingBankValue;
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
        if (betAmount < minimumBet)
        {
            Debug.Log("Must bet " + minimumBet.ToString() + " or more");
            return;
        }
        if (betAmount > phoneManager.BankValue)
        {
            Debug.Log("Cannot bet more than you have");
            return;
        }
        InitialDraw();

    }

    private void InitialDraw()
    {
        CreateDeck();
        drawn = false;
        SetState(BlackjackState.InitialDraw);
        currentBet = Convert.ToInt32(betInput.text);
        phoneManager.BankValue -= currentBet;
        HandStarted?.Invoke(currentBet);
        DrawPlayerCard();
        DrawPlayerCard();
        DrawDealerCard(false);
        DrawDealerCard(true);
        SetState(BlackjackState.Playing);
        if (dealerCardAceValue == 21)
        {
            if (cardAceValue == 21) EndGame(EndState.Draw);
            else EndGame(EndState.Loss);
        }
        else if (cardAceValue == 21) EndGame(EndState.NaturalWin);
        Debug.Log("Starting hand!");
    }

    private void CheckValidityOfDouble()
    {
        if (currentState == BlackjackState.Idle)
        {
            Debug.Log("Can only double while playing");
            return;
        }
        if (currentBet > phoneManager.BankValue)
        {
            Debug.Log("You do not have enough money to double");
            return;
        }
        Double();
    }

    private void Double()
    {
        phoneManager.BankValue -= currentBet;
        currentBet *= 2;
        DoubledDown?.Invoke(currentBet);
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
        CreateDisplayCard(card, true);
        UpdatePlayerScore();
    }

    private void DrawDealerCard(bool hidden)
    {
        //Check intoxication event
        Card card = cards[UnityEngine.Random.Range(0, cards.Count)];
        dealerDrawnCards.Add(card);
        cards.Remove(card);
        card.hidden = hidden;
        CreateDisplayCard(card, false);
        UpdateDealerScore();
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
        drawn = true;
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
        // Debug.Log("stand");
        RevealDealerCards();
        while (dealerCardAceValue < 17) DrawDealerCard(false);
        if (currentState != BlackjackState.Idle)
        {
            if (dealerCardAceValue < cardAceValue) EndGame(EndState.Win);
            else if (dealerCardAceValue > cardAceValue) EndGame(EndState.Loss);
            else EndGame(EndState.Draw);
        }
    }

    private int CountAces(int initialValue, int aces)
    {
        if (initialValue + aces > 21) return initialValue + aces;
        int value = initialValue;
        int currentAces = aces;
        while (currentAces > 0 && value < 21)
        {
            currentAces--;
            value += 11;
        }
        if (value > 21)
        {
            value -= 10;
            value += currentAces;
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
        cardValue = value + aces;
        if (aces > 0) value = CountAces(value, aces);
        cardAceValue = value;

        score.text = cardValue.ToString();
        if (cardAceValue != cardValue) score.text += " (" + cardAceValue.ToString() + ")";

        if (currentState == BlackjackState.Playing)
        {
            if (cardValue > 21) EndGame(EndState.Loss);
            else if (cardAceValue == 21) Stand();
        }

    }

    private void UpdateDealerScore()
    {
        int aces = 0;
        int value = 0;
        int hiddenCards = 0;
        for (int i = 0; i < dealerDrawnCards.Count; i++)
        {
            if (dealerDrawnCards[i] != null)
            {
                if (dealerDrawnCards[i].hidden)
                {
                    hiddenCards++;
                    continue;
                }
                else if (dealerDrawnCards[i].number >= 10)
                {
                    value += 10;
                }
                else if (dealerDrawnCards[i].number == 1)
                {
                    aces++;
                }
                else
                {
                    value += dealerDrawnCards[i].number;
                }
            }
        }
        dealerCardValue = value + aces;
        if (aces > 0) value = CountAces(value, aces);
        dealerCardAceValue = value;

        dealerScore.text = dealerCardValue.ToString();
        if (dealerCardAceValue != dealerCardValue) dealerScore.text += " (" + dealerCardAceValue.ToString() + ")";
        if (hiddenCards > 0) dealerScore.text += " + ?";

        if (currentState == BlackjackState.Playing)
        {
            if (dealerCardValue > 21) EndGame(EndState.Win);
            else if (dealerCardAceValue == 21 && cardAceValue < 21) EndGame(EndState.Loss);
        }
    }

    private void RevealDealerCards()
    {
        foreach (Card dealerCard in dealerDrawnCards)
        {
            dealerCard.hidden = false;
            dealerCard.obj.GetComponent<Image>().sprite = dealerCard.sprite;
        }
        UpdateDealerScore();
    }

    private void EndGame(EndState state)
    {
        switch (state)
        {
            case EndState.Win:
                {
                    Debug.Log("WIN");
                    foreach (Texture texture in chipParticles) StartCoroutine(particleManager.EmitParticles(30, 2, 5, texture, new Vector2(0, canvas.pixelRect.height + 100), new Vector2(canvas.pixelRect.width, canvas.pixelRect.height + 100), Vector2.zero, new Vector2(0, -1000), new Vector2(0, -300), new Vector2(0, -300),
                        Vector3.zero, new Vector3(360, 360, 360), new Vector3(-360,-360,-360), new Vector3(360,360,360), Vector3.zero, Vector3.zero));
                    phoneManager.BankValue += currentBet * 2;
                    score.color = winTextColour;
                    dealerScore.color = loseTextColour;
                    if (cardAceValue == 21)
                    {
                        feedbackText.text = "BLACKJACK";
                        feedbackText.color = naturalWinTextColour;
                        
                    }
                    else
                    {
                        feedbackText.text = "WIN";
                        feedbackText.color = winTextColour;
                    }
                    feedbackText.gameObject.SetActive(true);
                    if (dealerCardAceValue > 21)
                    {
                        dealerFeedbackText.text = "BUST";
                        dealerFeedbackText.color = loseTextColour;
                        dealerFeedbackText.gameObject.SetActive(true);
                    }
                    HandEnded?.Invoke(state, currentBet * 2);
                    StartCoroutine(HandStatusAnimation(currentBet * 2, "Won + $"));
                    break;
                }
            case EndState.Draw:
                {
                    Debug.Log("PUSH");
                    StartCoroutine(HandStatusAnimation(currentBet, "Push $"));
                    phoneManager.BankValue += currentBet;
                    score.color = drawTextColour;
                    dealerScore.color = drawTextColour;
                    feedbackText.text = "PUSH";
                    feedbackText.color = drawTextColour;
                    feedbackText.gameObject.SetActive(true);
                    HandEnded?.Invoke(state, currentBet);
                    break;
                }
            case EndState.Loss:
                {
                    Debug.Log("LOSS");
                    StartCoroutine(HandStatusAnimation(currentBet, "Lost - $"));
                    score.color = loseTextColour;
                    dealerScore.color = winTextColour;
                    if (cardAceValue > 21)
                    {
                        feedbackText.text = "BUST";
                    }
                    else
                    {
                        feedbackText.text = "LOSS";
                    }
                    feedbackText.color = loseTextColour;
                    feedbackText.gameObject.SetActive(true);
                    if (dealerCardAceValue == 21)
                    {
                        dealerFeedbackText.text = "BLACKJACK";
                        dealerFeedbackText.color = naturalWinTextColour;
                        dealerFeedbackText.gameObject.SetActive(true);
                    }
                    HandEnded?.Invoke(state, -currentBet);
                    break;
                }
            case EndState.NaturalWin:
                {
                    Debug.Log("NATURAL WIN");
                    foreach (Texture texture in chipParticles) StartCoroutine(particleManager.EmitParticles(20, 2, 5, texture, new Vector2(0, canvas.pixelRect.height + 100), new Vector2(canvas.pixelRect.width, canvas.pixelRect.height + 100), Vector2.zero, new Vector2(0, -1000), new Vector2(0, -300), new Vector2(0, -300),
                        Vector3.zero, new Vector3(360, 360, 360), new Vector3(-360, -360, -360), new Vector3(360, 360, 360), Vector3.zero, Vector3.zero));
                    phoneManager.BankValue += currentBet * 2;
                    phoneManager.BankValue += (int)Mathf.Floor(currentBet * 1.5f);
                    StartCoroutine(HandStatusAnimation((int)Mathf.Floor(currentBet * 1.5f), "Blackjack won + $"));
                    score.color = naturalWinTextColour;
                    dealerScore.color = loseTextColour;
                    feedbackText.text = "NAT WIN";
                    feedbackText.color = naturalWinTextColour;
                    feedbackText.gameObject.SetActive(true);
                    HandEnded?.Invoke(state, (int)Mathf.Floor(currentBet * 1.5f));
                    break;
                }
        }
        currentBet = 0;
        SetState(BlackjackState.Idle);
        EndOfHand.Invoke();
    }

    private IEnumerator HandStatusAnimation(int betValue, string winCondition)
    {
        // Debug.Log("Coroutine Started");
        statusFieldText.gameObject.SetActive(true);
        RectTransform statusRect = statusFieldText.GetComponent<RectTransform>();
        float elapsedMoveTime = 0f;
        float moveDuration = 2f;
        Vector2 startPosition = statusRect.position;
        statusFieldText.text = winCondition + betValue + "!";

        while (elapsedMoveTime < moveDuration)
        {
            statusRect.position = new Vector2(startPosition.x, startPosition.y - 25f * Time.deltaTime);
            elapsedMoveTime += Time.deltaTime;
            yield return null;
        }

        statusFieldText.gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawButton.onClick.AddListener(CheckValidityOfDraw);
        doubleButton.onClick.AddListener(CheckValidityOfDouble);
        hitButton.onClick.AddListener(CheckValidityOfHit);
        standButton.onClick.AddListener(CheckValidityOfStand);
        ResetState();
        statusFieldText.gameObject.SetActive(false);
    }
}

[Serializable]
public class Card
{
    public BlackjackManager.Suit suit;
    public int number;
    public bool hidden;
    public GameObject obj;
    public Sprite sprite;
}