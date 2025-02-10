using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NoFaceScript : MonoBehaviour
{
    public TextMeshProUGUI loadingTxt;
    public Button tryAgainBtn;
    void Start()
    {
        tryAgainBtn.onClick.AddListener(onClickTryAgain);
    }

    // Update is called once per frame
    void Update()
    {
        if (SetupScript.instance.nfdText != null && !string.IsNullOrEmpty(SetupScript.instance.nfdText))
        {
            loadingTxt.text = SetupScript.instance.nfdText;
        }
    }

    public void onClickTryAgain() {
        SceneManager.LoadScene("cameraScene", LoadSceneMode.Single);
    }
}
