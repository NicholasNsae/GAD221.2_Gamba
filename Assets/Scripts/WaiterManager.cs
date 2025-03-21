using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaiterManager : MonoBehaviour
{
    [SerializeField] private GameObject waiterPanel;
    [SerializeField] private Button waiterAcceptDrinkButton;
    [SerializeField] private Button waiterDeclineDrinkButton;
    [SerializeField] private TextMeshProUGUI waiterNotificationText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waiterPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // THIS FUNCTION WILL BE ADDED TO TIME EVENT
    public void WaiterNotify()
    {
        int activateWaiter = Random.Range(0, 4);

        if (activateWaiter == 0)
        {
            waiterPanel.gameObject.SetActive(true);
            waiterAcceptDrinkButton.onClick.AddListener(AcceptedWaiterRequestButton);
            waiterDeclineDrinkButton.onClick.AddListener(DeclineWaiterRequestButton);
        }
    }

    private void AcceptedWaiterRequestButton()
    {
        waiterNotificationText.text = "Drink = $25";
        Text acceptText = waiterAcceptDrinkButton.GetComponentInChildren<Text>();
        acceptText.text = "Ok";
    }

    private void DeclineWaiterRequestButton()
    {
        waiterAcceptDrinkButton.onClick.RemoveAllListeners();
        waiterDeclineDrinkButton.onClick.RemoveAllListeners();
        waiterPanel.gameObject.SetActive(false);
    }
}
