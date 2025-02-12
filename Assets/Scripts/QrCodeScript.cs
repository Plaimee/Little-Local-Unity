using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using ZXing;
using ZXing.QrCode;

public class QRCodeScript : MonoBehaviour
{
    public static QRCodeScript instance;
    public GameObject[] little;
    public string littleSelected;
    public RawImage orgImage;
    public string orgImagePath;
    public RawImage location;
    public string locationSelected;
    public RawImage stamp;
    public string stampSelected;
    public TextMeshProUGUI outputIdText;
    public string outputId;
    public RawImage qrCode;
    public string qrUrl;
    public string fullQrUrl;
    private bool hasInitialized = false;

    void Start()
    {
        instance = this;
        InitializeQRCode();
    }

    private void InitializeQRCode()
    {
        if (string.IsNullOrEmpty(SetupScript.instance.outputId))
        {
            Debug.LogError("Output ID is null or empty");
            return;
        }

        qrUrl = "https://funcslash.com/projects/2025/lclm/get.php?noId=";
        orgImagePath = WebcamScript.instance.imgPath;
        locationSelected = SelectLocationScript.instance.locationImage;
        stampSelected = SelectLocationScript.instance.stampImage;
        littleSelected = CharacterScript.instance.btnName;
        outputId = SetupScript.instance.outputId;
        fullQrUrl = qrUrl + outputId;

        GenerateQrCodeFromPath(fullQrUrl);
        ShowImage(orgImage, orgImagePath);
        ShowImage(location, locationSelected);
        ShowImage(stamp, stampSelected);
        DeactivateAllLittle();
        ActivateMatchingLittle();

        if(!string.IsNullOrEmpty(outputId))
        {
            outputIdText.text = outputId;
            Debug.Log($"Set output ID text: {outputId}");
            StartCoroutine(ResetSocketAfterDelay());
        }

        hasInitialized = true;
    }

    private IEnumerator ResetSocketAfterDelay()
    {
        // รอให้ QR Code และองค์ประกอบอื่นๆ แสดงผลเสร็จ
        yield return new WaitForSeconds(2f);
        
        if (SetupScript.instance != null)
        {
            SetupScript.instance.hasResetAfterQR = true;
            SetupScript.instance.ResetSocketData();
            Debug.Log("Reset socket data after QR Code generation");
        }
    }
    void DeactivateAllLittle()
    {
        foreach (GameObject obj in little)
        {
            obj.SetActive(false);
        }
        Debug.Log("All little GameObjects are set to inactive.");
    }

    void ActivateMatchingLittle()
    {
        GameObject matchingObject = little.FirstOrDefault(obj => obj.name == littleSelected);
        
        if (matchingObject != null)
        {
            matchingObject.SetActive(true);
            Debug.Log("Activated: " + matchingObject.name);
        }
        else
        {
            Debug.LogWarning("No matching GameObject found for: " + littleSelected);
        }
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

    void GenerateQrCodeFromPath(string urlPath) {
        Texture2D qrCodeTexture = GenerateQrCode(urlPath, 256, 256);
        qrCode.texture = qrCodeTexture;
    }

    public Texture2D GenerateQrCode(string path, int width, int height) {
        BarcodeWriter writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Width = width,
                Height = height,
            }
        };
        Color32[] pixels = writer.Write(path);
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels32(pixels);
        texture.Apply();
        return texture;
    }
}
