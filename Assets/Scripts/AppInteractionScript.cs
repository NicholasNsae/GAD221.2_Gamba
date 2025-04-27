using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AppInteractionScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image appImage;

    [SerializeField] private GameObject bankApp;
    [SerializeField] private GameObject messagerApp;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        appImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (appImage.name == "BankAppImage")
        {
            bankApp.SetActive(true);
            messagerApp.SetActive(false);
        }
        else if (appImage.name == "MessagerAppImage")
        {
            messagerApp.SetActive(true);
            bankApp.SetActive(false);
        }
    }
}
