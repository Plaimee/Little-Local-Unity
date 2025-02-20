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
    public GameObject dp2Parent;
    public GameObject photo1;
    public GameObject photo2;
    public RawImage output;
    public string outputPath;
    public string oldOutputPath;
    public Animator photo1Animator;
    public Animator photo2Animator;
    public Animator outputAnimator;
    public bool check = true;

    // เพิ่ม reference สำหรับ CanvasGroup
    private CanvasGroup canvasGroup;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        if(Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
        output.gameObject.SetActive(false);

        // ดึง หรือเพิ่ม CanvasGroup component
        canvasGroup = dp2Parent.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dp2Parent.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if(AllImagesExist()) {
            outputPath = OutputFourthWithFrameScript.instance.saveImagePath;
            if(outputPath != oldOutputPath) {
                oldOutputPath = outputPath;
                output.gameObject.SetActive(false);
                StartCoroutine(RepeatedMoveAnimation());
            }
        }

        if (SetupScript.instance != null && 
            Time.time - SetupScript.instance.lastInteractionTime >= SetupScript.instance.inactivityTimeout)
        {
            ResetAllAnimations();
        }
    }

    private bool AllImagesExist()
    {
        return !string.IsNullOrEmpty(OutputFirstRawScript.instance.capImagePath) &&
               !string.IsNullOrEmpty(OutputFirstWithFrameScript.instance.saveImagePath) &&
               !string.IsNullOrEmpty(OutputSecondRawScript.instance.capImagePath) &&
               !string.IsNullOrEmpty(OutputSecondWithFrameScript.instance.saveImagePath) &&
               !string.IsNullOrEmpty(OutputThirdRawScript.instance.capImagePath) &&
               !string.IsNullOrEmpty(OutputThirdWithFrameScript.instance.saveImagePath) &&
               !string.IsNullOrEmpty(OutputFourthWithFrameScript.instance.saveImagePath) &&
               File.Exists(OutputFirstRawScript.instance.capImagePath) &&
               File.Exists(OutputFirstWithFrameScript.instance.saveImagePath) &&
               File.Exists(OutputSecondRawScript.instance.capImagePath) &&
               File.Exists(OutputSecondWithFrameScript.instance.saveImagePath) &&
               File.Exists(OutputThirdRawScript.instance.capImagePath) &&
               File.Exists(OutputThirdWithFrameScript.instance.saveImagePath) &&
               File.Exists(OutputFourthWithFrameScript.instance.saveImagePath);
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
        ShowImage(output, outputPath);
        output.gameObject.SetActive(true);
        outputAnimator.SetTrigger("PlayOutputAnimation");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeInEffect());
        check = true;
    }

    private IEnumerator FadeInEffect()
    {
        // เริ่มต้นด้วยการเปิด GameObject และตั้งค่า alpha เป็น 0
        dp2Parent.SetActive(true);
        canvasGroup.alpha = 0f;

        float elapsedTime = 0f;
        float duration = 1f;

        // ค่อยๆ เพิ่มค่า alpha
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private void ResetAllAnimations()
    {
        StopAllCoroutines();

        if (photo1Animator != null)
        {
            photo1Animator.ResetTrigger("PlayPhoto1Animation");
            photo1Animator.SetTrigger("ReturnToIdle");
        }

        if (photo2Animator != null)
        {
            photo2Animator.ResetTrigger("PlayPhoto2Animation");
            photo2Animator.SetTrigger("ReturnToIdle");
        }

        if (outputAnimator != null)
        {
            outputAnimator.ResetTrigger("PlayOutputAnimation");
            outputAnimator.SetTrigger("ReturnToIdle");
        }

        if (output != null)
        {
            output.gameObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        check = true;
        oldOutputPath = null;
    }
}