using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WebcamScript : MonoBehaviour
{
    public static WebcamScript instance;
    public RawImage camTexture;
    public TextMeshProUGUI timerText;
    public Image thirdStage;
    public Sprite changeStage;
    public Sprite defaultStage;
    public Button captureBtn;
    public Button confirmBtn;
    public Button retakeBtn;
    public Button backBtn;
    public string imgPath;
    public string imgDir = "E:\\work\\BKKDW2024\\photo\\";

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        captureBtn.onClick.AddListener(StartCountdown);
        confirmBtn.onClick.AddListener(ConfirmCapture);
        retakeBtn.onClick.AddListener(RetakePhoto);
        backBtn.onClick.AddListener(onClickBackBtn);
        captureBtn.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        retakeBtn.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        SetupScript.instance.StartCamera();
    }

    // Update is called once per frame
    void Update()
    {
        camTexture.texture = SetupScript.instance.webCamTexture;
    }

    void ConfirmCapture()
    {
        SaveImage();
        SetupScript.instance.StopCamera();
        SceneManager.LoadScene("outputScene", LoadSceneMode.Single);
    }

    void CaptureImage()
    {
        camTexture.texture = SetupScript.instance.webCamTexture;
        camTexture.gameObject.SetActive(true);
        SetupScript.instance.PauseCamera();

        captureBtn.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(true);
        retakeBtn.gameObject .SetActive(true);
    }

    void RetakePhoto()
    {
        // ปิดใช้งานปุ่ม Retake
        retakeBtn.gameObject.SetActive(false);

        // แสดงกล้องและเริ่มนับถอยหลังใหม่
        camTexture.gameObject.SetActive(true);
        thirdStage.sprite = defaultStage;
        captureBtn.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        confirmBtn.gameObject.SetActive(false);
        SetupScript.instance.StartCamera();
    }

    void SaveImage()
    {
        WebCamTexture webcamTexture = SetupScript.instance.webCamTexture;
        Texture2D webcamTexture2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        webcamTexture2D.SetPixels(webcamTexture.GetPixels());
        webcamTexture2D.Apply();

        byte[] bytes = webcamTexture2D.EncodeToPNG();

        imgPath = imgDir + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".png";
        File.WriteAllBytes(imgPath, bytes);
        Debug.Log("Full image saved at: " + imgPath);

        // Emit the image path if the file exists
        if (File.Exists(imgPath))
        {
            SetupScript.instance.socket.Emit("imgPath", imgPath);
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
        timerText.gameObject.SetActive(true);
        captureBtn.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(false);
    }

    IEnumerator CountdownCoroutine()
    {
        for (int i = 3; i >= 0; i--)
        {
            timerText.text = i.ToString();
            if (i == 0)
            {
                timerText.text = "";
            }

            yield return new WaitForSecondsRealtime(1);
        }

        yield return new WaitForSecondsRealtime(0.1f);
        CaptureImage();
        ChangeStageImage();

        confirmBtn.gameObject.SetActive(true);
        retakeBtn.gameObject.SetActive(true);
    }

    public void ChangeStageImage()
    {
        thirdStage.sprite = changeStage;
    }

    public void onClickBackBtn()
    {
        SceneManager.LoadScene("selectLocationScene", LoadSceneMode.Single);
    }
}
