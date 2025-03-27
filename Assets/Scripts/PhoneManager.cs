using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private int bankValue;

    public int BankValue {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BankValue = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
