using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ProcessScripts : MonoBehaviour
{
    public static ProcessScripts instance;
    public RawImage orgImage;
    public string orgImagePath;
    private bool sceneLoadTriggered = false;
    
    void Start()
    {
        instance = this;     
        orgImagePath = WebcamScript.instance.imgPath;
        ShowImage(orgImage, orgImagePath);
    }

    void Update()
    {
        if (!sceneLoadTriggered && AllImagesExist())
        {
            sceneLoadTriggered = true;
            StartCoroutine(LoadSceneWithDelay(3f));
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

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        Debug.Log("All images found, loading scene in " + delay + " seconds...");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("qrCodeScene", LoadSceneMode.Single);
    }

    public void ShowImage(RawImage imageContainer, string imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
        {
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
        else
        {
            Debug.LogError("Image path is null, empty, or file does not exist.");
        }
    }
}
