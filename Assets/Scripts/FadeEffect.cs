using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    public GameObject targetObject; // Assign the GameObject to fade in/out

    private Renderer objectRenderer;
    private SpriteRenderer spriteRenderer;
    public bool isUIElement = false;

    void Start()
    {
        if (targetObject == null)
            targetObject = gameObject; // Default to the current GameObject

        objectRenderer = targetObject.GetComponent<Renderer>();
        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (targetObject.GetComponent<CanvasGroup>() != null)
        {
            isUIElement = true;
            Debug.LogWarning("Target has a CanvasGroup! Consider using CanvasGroup instead.");
        }
    }

    public void FadeIn(float duration)
    {
        if (objectRenderer || spriteRenderer)
            StartCoroutine(FadeGameObject(0, 1, duration));
    }

    public void FadeOut(float duration)
    {
        if (objectRenderer || spriteRenderer)
            StartCoroutine(FadeGameObject(1, 0, duration));
    }

    private IEnumerator FadeGameObject(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetObjectAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetObjectAlpha(endAlpha);
    }

    private void SetObjectAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        else if (objectRenderer != null)
        {
            foreach (Material mat in objectRenderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }
}
