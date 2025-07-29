using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenTint : MonoBehaviour
{
    public Image tintImage;
    public float tintDuration = 0.22f;
    public Color tintColor = new Color(1, 0, 0, 0.13f);

    private Coroutine tintCoroutine;

    private void Awake()
    {
        if (tintImage == null)
            tintImage = GetComponent<Image>();
        tintImage.color = new Color(tintColor.r, tintColor.g, tintColor.b, 0f); 
    }

    public void Flash()
    {
        if (tintCoroutine != null)
            StopCoroutine(tintCoroutine);
        tintCoroutine = StartCoroutine(FlashTint());
    }

    IEnumerator FlashTint()
    {
        tintImage.color = tintColor;

        float timer = 0f;
        while (timer < tintDuration)
        {
            // Fade out alpha over time
            float alpha = Mathf.Lerp(tintColor.a, 0f, timer / tintDuration);
            tintImage.color = new Color(tintColor.r, tintColor.g, tintColor.b, alpha);
            timer += Time.unscaledDeltaTime; 
            yield return null;
        }
        tintImage.color = new Color(tintColor.r, tintColor.g, tintColor.b, 0f);
    }
}
