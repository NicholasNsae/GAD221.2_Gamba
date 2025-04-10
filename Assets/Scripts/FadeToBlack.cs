using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    // TODO - Find references in awake if not set
    [SerializeField] private EventManager eventManager;
    [SerializeField] private Image ImageToFade;
    [SerializeField] private TextMeshProUGUI TitleTextToFade;
    [SerializeField] private TextMeshProUGUI BodyTextToFade;
    
    [SerializeField] private float imageFadeInAndOutDuration = 0.5f;
    [SerializeField] private float textFadeInAndOutDuration = 0.5f;
    [SerializeField] private float noTextStayFadedDuration = 0f;
    [SerializeField] private float textVisibleForDuration = 3f;

    private float timer;
    private bool currentlyFading;
    public bool CurrentlyFading => currentlyFading;
    
    //TODO - Turn off controls!
    public IEnumerator FadeToBlackCoroutine(EventDetails eventDetails)
    {
        gameObject.SetActive(true);
        timer = 0f;
        BodyTextToFade.text = "";
        TitleTextToFade.text = "";
        
        currentlyFading = true;
        
        yield return StartCoroutine(FadeImage(1f, imageFadeInAndOutDuration));
        
        string[] bodyText = eventDetails.EventBodyText;
        string titleText = eventDetails.EventTitleText;
        
        if (bodyText.Length == 0 && string.IsNullOrEmpty(titleText))
        {
            yield return StartCoroutine(WaitForTime(noTextStayFadedDuration));
        }
        
        else if (bodyText.Length > 0 && !string.IsNullOrEmpty(titleText))
        {
            TitleTextToFade.text = titleText;

            for (var i = 0; i < bodyText.Length; i++)
            {
                var message = bodyText[i];
                BodyTextToFade.text = message;
                
                if (i == 0)
                {
                    yield return StartCoroutine(FadeBodyAndTitleText(1f, textFadeInAndOutDuration));
                }
                else
                {
                    yield return StartCoroutine(FadeBodyText(1f, textFadeInAndOutDuration));
                }
                
                yield return StartCoroutine(WaitForTime(textVisibleForDuration));

                if (i == bodyText.Length - 1)
                {
                    yield return StartCoroutine(FadeBodyAndTitleText(0f, textFadeInAndOutDuration));
                }
                else
                {
                    yield return StartCoroutine(FadeBodyText(0f, textFadeInAndOutDuration));
                }
            }
        }
        
        else if (bodyText.Length > 0 && string.IsNullOrEmpty(titleText))
        {
            foreach (string message in bodyText)
            {
                BodyTextToFade.text = message;
                yield return StartCoroutine(FadeBodyText(1f, textFadeInAndOutDuration));
                yield return StartCoroutine(WaitForTime(textVisibleForDuration));
                yield return StartCoroutine(FadeBodyText(0f, textFadeInAndOutDuration));
            }
        }
        
        else if (bodyText.Length == 0 && !string.IsNullOrEmpty(titleText))
        {
            TitleTextToFade.text = titleText;
            yield return StartCoroutine(FadeTitleText(1f, textFadeInAndOutDuration));
            yield return StartCoroutine(WaitForTime(textVisibleForDuration));
            yield return StartCoroutine(FadeTitleText(0f, textFadeInAndOutDuration));
        }
        
        yield return StartCoroutine(FadeImage(0f, imageFadeInAndOutDuration));
        
        currentlyFading = false;
        gameObject.SetActive(false);
    }

    public IEnumerator FadeImage(float targetAlpha, float duration)
    {
        Color color = ImageToFade.color;
        float startingAlpha = color.a;

        while (timer < imageFadeInAndOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startingAlpha, targetAlpha, timer / duration);
            ImageToFade.color = color;
            yield return null;
        }
        
        timer -= duration;
        ImageToFade.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    public IEnumerator FadeBodyText(float targetAlpha, float duration)
    {
        Color color = BodyTextToFade.color;
        float startingAlpha = color.a;

        while (timer < imageFadeInAndOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startingAlpha, targetAlpha, timer / duration);
            BodyTextToFade.color = color;
            yield return null;
        }
        
        timer -= duration;
        BodyTextToFade.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    public IEnumerator FadeTitleText(float targetAlpha, float duration)
    {
        Color color = TitleTextToFade.color;
        float startingAlpha = color.a;

        while (timer < imageFadeInAndOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startingAlpha, targetAlpha, timer / duration);
            TitleTextToFade.color = color;
            yield return null;
        }
        
        timer -= duration;
        TitleTextToFade.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    public IEnumerator FadeBodyAndTitleText(float targetAlpha, float duration)
    {
        Color titleColor = TitleTextToFade.color;
        float startingTitleAlpha = titleColor.a;
        
        Color bodyColor = BodyTextToFade.color;
        float startingBodyAlpha = bodyColor.a;

        while (timer < imageFadeInAndOutDuration)
        {
            timer += Time.deltaTime;
            titleColor.a = Mathf.Lerp(startingTitleAlpha, targetAlpha, timer / duration);
            bodyColor.a = Mathf.Lerp(startingBodyAlpha, targetAlpha, timer / duration);
            TitleTextToFade.color = titleColor;
            BodyTextToFade.color = bodyColor;
            yield return null;
        }
        
        timer -= duration;
        TitleTextToFade.color = new Color(titleColor.r, titleColor.g, titleColor.b, targetAlpha);
        BodyTextToFade.color = new Color(bodyColor.r, bodyColor.g, bodyColor.b, targetAlpha);
    }

    public IEnumerator WaitForTime(float duration)
    {
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        timer -= duration;
    }
}
