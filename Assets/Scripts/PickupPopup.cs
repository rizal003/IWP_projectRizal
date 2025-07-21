using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public Image iconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI statInfoText;

    private CanvasGroup popupCanvasGroup;

    void Awake()
    {
        if (popupPanel != null)
            popupCanvasGroup = popupPanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = 0;
        }
    }

    public void Show(Sprite icon, string itemName, string statInfo)
    {
        Debug.Log("PickupPopup.Show() called!");
        popupPanel.SetActive(true);
        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0;
        iconImage.sprite = icon;
        itemNameText.text = itemName;
        statInfoText.text = statInfo;

        StopAllCoroutines();
        StartCoroutine(FadeInPopup(0.3f));
        StartCoroutine(HideAfterDelay(3.5f)); // Popup lasts for 2s + 0.2s fadeout
    }

    IEnumerator FadeInPopup(float fadeTime)
    {
        float t = 0f;
        if (popupCanvasGroup == null) yield break;
        while (t < fadeTime)
        {
            popupCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        popupCanvasGroup.alpha = 1;
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay - 0.2f); // 0.2s before fade out
        if (popupCanvasGroup != null)
        {
            float t = 0f;
            float fadeTime = 0.2f;
            while (t < fadeTime)
            {
                popupCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
                t += Time.deltaTime;
                yield return null;
            }
            popupCanvasGroup.alpha = 0;
        }
        popupPanel.SetActive(false);
    }
}
