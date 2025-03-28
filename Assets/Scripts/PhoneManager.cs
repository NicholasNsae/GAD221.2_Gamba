using TMPro;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bankValueText;
    [SerializeField] private int bankValue;

    public int BankValue {get; set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BankValue = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        bankValueText.text = "Account Amount: $" + BankValue.ToString();
    }
}
