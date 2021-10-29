using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    Image fadeImage;
    float targetAlpha;
    float fadeSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        //Get Reference 
        fadeImage = GetComponent<Image>();
        targetAlpha = fadeImage.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor = fadeImage.color;

        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);

        fadeImage.color = currentColor;
    }

    public void FadeToWhite()
    {
        targetAlpha = 1.0f;
    }

    public void FadeIn()
    {
        targetAlpha = 0.0f;
    }
}
