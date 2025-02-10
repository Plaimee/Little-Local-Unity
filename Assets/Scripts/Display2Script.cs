using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Display2Script : MonoBehaviour
{
    private static Display2Script instance;
    public Camera display2;
    public GameObject photo1;
    public GameObject photo2;
    public RawImage output;
    public string outputPath;
    public string oldOutputPath;
    public Animator photo1Animator;
    public Animator photo2Animator;
    public Animator outputAnimator;
    public bool check = true;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        if(Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
        output.gameObject.SetActive(false);
    }

    void Update()
    {
        if(FourthOutputScript.instance != null && !string.IsNullOrEmpty(FourthOutputScript.instance.saveImagePath)) {
            outputPath = FourthOutputScript.instance.saveImagePath;
            if(outputPath != oldOutputPath) {
                oldOutputPath = outputPath;
                output.gameObject.SetActive(false);
                StartCoroutine(RepeatedMoveAnimation());
            }
        } 
    }

    public void ShowImage(RawImage imageContainer, string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Image path is null or empty.");
            return;
        }

        if (!File.Exists(imagePath))
        {
            Debug.LogError("File does not exist at path: " + imagePath);
            return;
        }

        Debug.Log("Loading image from: " + imagePath);
        byte[] imageData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            imageContainer.texture = texture;
            imageContainer.GetComponent<AspectRatioFitter>().aspectRatio = texture.width / (float)texture.height;
            Debug.Log("Image loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load image from file: " + imagePath);
        }
    }

    private IEnumerator RepeatedMoveAnimation()
    {
        check = false;
        photo1Animator.SetTrigger("PlayPhoto1Animation");
        photo2Animator.SetTrigger("PlayPhoto2Animation");
        yield return new WaitForSeconds(3f);
        output.gameObject.SetActive(true);
        outputAnimator.SetTrigger("PlayOutputAnimation");
        yield return new WaitForSeconds(1.5f);
        ShowImage(output, outputPath);
        StartCoroutine(FadeInEffect());
        check = true;
    }

    private IEnumerator FadeInEffect()
    {
        output.gameObject.SetActive(true); // Ensure the output is visible

        Color color = output.color; // Get current color
        float elapsedTime = 0f;
        float duration = 1f; // 1 second fade-in duration

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration); // Gradually increase alpha
            output.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f; // Ensure it's fully visible
        output.color = color;
    }

}
