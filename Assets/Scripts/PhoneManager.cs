using System;
using TMPro;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bankValueText;
    [SerializeField] private int bankValue;
    [SerializeField] private int startingBankValue = 1000;
    public int StartingBankValue => startingBankValue;
    
    public event Action<int, int> BankValueChanged; // First int is old value, second int is new value

    public int BankValue
    {
        get => bankValue;
        set
        {
            BankValueChanged?.Invoke(bankValue, value);
            // UpdateBankText();
            bankValue = value;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BankValue = startingBankValue;
    }

    // // I added this so it doesn't update every frame (but it won't update if the bank value is changed in the inspector) - DE
    // private void UpdateBankText()
    // {
    //     bankValueText.text = "Account Amount: $" + BankValue.ToString();
    // }
    
    // Update is called once per frame
    void Update()
    {
        bankValueText.text = "Account Amount: $" + BankValue.ToString();
    }
}
