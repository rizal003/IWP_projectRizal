using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyCounterUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public TextMeshProUGUI keyText;
    public Image keyIcon;

    private void Start()
    {
        // Always try to find the persistent inventory
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        if (playerInventory != null)
        {
            // Remove listener first, just in case, to prevent duplicates
            playerInventory.OnKeyCountChanged.RemoveListener(OnKeyCountChanged);
            playerInventory.OnKeyCountChanged.AddListener(OnKeyCountChanged);
            UpdateKeyUI(playerInventory.keyCount, false);
        }
    }

    private void OnKeyCountChanged(int count)
    {
        UpdateKeyUI(count, true);
    }

    public void UpdateKeyUI(int count, bool animate = true)
    {
        keyText.text = count.ToString("00");
        if (animate)
            AnimateKeyPop();
    }

    public void AnimateKeyPop()
    {
        StartCoroutine(KeyPopCoroutine());
    }

    IEnumerator KeyPopCoroutine()
    {
        // Only affect text
        Vector3 origTextScale = keyText.transform.localScale;
        Color origColor = keyText.color;

        // Make text bigger and yellow
        keyText.transform.localScale = origTextScale * 1.2f;
        keyText.color = Color.yellow;

        // Hold for a short moment
        yield return new WaitForSeconds(0.15f);

        // Animate back to normal
        float t = 0f;
        float duration = 0.4f;
        while (t < duration)
        {
            float lerp = t / duration;
            keyText.transform.localScale = Vector3.Lerp(origTextScale * 1.2f, origTextScale, lerp);
            keyText.color = Color.Lerp(Color.yellow, origColor, lerp);
            t += Time.deltaTime;
            yield return null;
        }
        keyText.transform.localScale = origTextScale;
        keyText.color = origColor;
    }
}
