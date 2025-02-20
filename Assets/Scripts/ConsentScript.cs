using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConsentScript : MonoBehaviour
{
    public GameObject webcamGroup;
    public GameObject consentGroup;
    public Button cancleBtn;
    public Button confirmBtn;
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        webcamGroup.SetActive(false);
        toggle.isOn = false;
        confirmBtn.interactable = false;

        toggle.onValueChanged.AddListener(OnToggleChanged);
        confirmBtn.onClick.AddListener(OnClickconfirmBtn);
        cancleBtn.onClick.AddListener(OnClickcancleBtn);
    }

    private void OnToggleChanged(bool isOn)
    {
        // Enable/disable next button based on toggle state
        confirmBtn.interactable = isOn;
    }

    // Handle next button click
    private void OnClickconfirmBtn()
    {
        // Switch visibility of groups
        webcamGroup.SetActive(true);
        consentGroup.SetActive(false);
    }

    public void OnClickcancleBtn() {
        SceneManager.LoadScene("selectLocationScene" ,LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
        confirmBtn.onClick.RemoveListener(OnClickconfirmBtn);
    }
}
