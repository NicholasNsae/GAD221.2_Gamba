using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class WaiterManager : MonoBehaviour
{
    [SerializeField] private PhoneManager phoneManager;
    
    [SerializeField] private GameObject waiterPanel;
    [SerializeField] private Button waiterAcceptDrinkButton;
    [SerializeField] private Button waiterDeclineDrinkButton;
    [SerializeField] private TextMeshProUGUI waiterNotificationText;
    [SerializeField] private int valueOfDrink;

    [Tooltip("UnityEvents")]
    [SerializeField] public UnityEvent onWaiterAcceptDrinkEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waiterPanel.gameObject.SetActive(false);
        WaiterNotifyEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // THIS FUNCTION WILL BE ADDED TO TIME EVENT
    public void WaiterNotifyEvent()
    {
        int activateWaiter = 0;//Random.Range(0, 4);

        if (activateWaiter == 0)
        {
            waiterPanel.gameObject.SetActive(true);
            waiterAcceptDrinkButton.gameObject.SetActive(true);
            waiterDeclineDrinkButton.gameObject.SetActive(true);
            waiterNotificationText.text = "Any Drinks Today?";
            TextMeshProUGUI acceptText = waiterAcceptDrinkButton.GetComponentInChildren<TextMeshProUGUI>();
            acceptText.text = "Yes";
            TextMeshProUGUI declineText = waiterDeclineDrinkButton.GetComponentInChildren<TextMeshProUGUI>();
            declineText.text = "No";
            waiterAcceptDrinkButton.onClick.RemoveAllListeners();
            waiterDeclineDrinkButton.onClick.RemoveAllListeners();
            waiterAcceptDrinkButton.onClick.AddListener(AcceptedWaiterRequestButton);
            waiterDeclineDrinkButton.onClick.AddListener(DeclineWaiterRequestButton);
        }
    }

    private void AcceptedWaiterRequestButton()
    {
        valueOfDrink = 20;
        waiterNotificationText.text = "Drink = $" + valueOfDrink;

        TextMeshProUGUI acceptText = waiterAcceptDrinkButton.GetComponentInChildren<TextMeshProUGUI>();
        acceptText.text = "Ok!";
        TextMeshProUGUI declineText = waiterDeclineDrinkButton.GetComponentInChildren<TextMeshProUGUI>();
        declineText.text = "To Much Thanks!";
        waiterAcceptDrinkButton.onClick.RemoveAllListeners();
        waiterAcceptDrinkButton.onClick.AddListener(AcceptDrinkButton);
        waiterDeclineDrinkButton.onClick.RemoveAllListeners();
        waiterDeclineDrinkButton.onClick.AddListener(DeclineDrinkButton);
    }

    private void DeclineWaiterRequestButton()
    {
        waiterAcceptDrinkButton.onClick.RemoveAllListeners();
        waiterDeclineDrinkButton.onClick.RemoveAllListeners();
        waiterPanel.gameObject.SetActive(false);
    }

    private void AcceptDrinkButton()
    {
        if (phoneManager.BankValue >= valueOfDrink)
        {
            // invoke drinking event
            onWaiterAcceptDrinkEvent.Invoke();
            waiterNotificationText.text = "Here you are, Have a good night!";
            waiterAcceptDrinkButton.onClick.RemoveAllListeners();
            waiterDeclineDrinkButton.onClick.RemoveAllListeners();
            waiterAcceptDrinkButton.gameObject.SetActive(false);
            waiterDeclineDrinkButton.gameObject.SetActive(false);
            StartCoroutine(FadeOutOfWaiter(5f));
        }
        else
        {
            waiterNotificationText.text = "Sorry but you do not have enough money to buy this drink.";
            waiterAcceptDrinkButton.onClick.RemoveAllListeners();
            waiterDeclineDrinkButton.onClick.RemoveAllListeners();
            waiterAcceptDrinkButton.gameObject.SetActive(false);
            waiterDeclineDrinkButton.gameObject.SetActive(false);
            StartCoroutine(FadeOutOfWaiter(5f));
        }
    }

    private void DeclineDrinkButton()
    {
        waiterNotificationText.text = "No Worries, Have Fun!";
        waiterAcceptDrinkButton.onClick.RemoveAllListeners();
        waiterDeclineDrinkButton.onClick.RemoveAllListeners();
        waiterAcceptDrinkButton.gameObject.SetActive(false);
        waiterDeclineDrinkButton.gameObject.SetActive(false);
        StartCoroutine(FadeOutOfWaiter(5f));
    }

    private IEnumerator FadeOutOfWaiter(float duration)
    {
        CanvasGroup waiterPanelCanvasGroup = waiterPanel.GetComponent<CanvasGroup>();
        float startAlpha = waiterPanelCanvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            waiterPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }

        waiterPanelCanvasGroup.alpha = 1;
        waiterPanel.gameObject.SetActive(false);
    }
}
