using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnCtrlScript : MonoBehaviour
{
    Scene activeScene;
    public string sceneName;
    public Button nextBtn;
    public Button backBtn;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();
        sceneName = activeScene.name;
        Debug.Log(sceneName);

        nextBtn.onClick.AddListener(onClickNextBtn);
        backBtn.onClick.AddListener(onClickBackBtn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickNextBtn()
    {
        switch (sceneName)
        {
            case "startScene":
                SceneManager.LoadScene("selectCharacterScene", LoadSceneMode.Single);
                break;

            case "selectCharacterScene":
                SceneManager.LoadScene("selectLocationScene", LoadSceneMode.Single);
                break;

            case "selectLocationScene":
                SceneManager.LoadScene("previewScene", LoadSceneMode.Single);
                break;
            
            case "previewScene":
                SceneManager.LoadScene("cameraScene", LoadSceneMode.Single);
                break;
        }
        
    }
    
    public void onClickBackBtn()
    {
        switch (sceneName)
        {
            case "selectCharacterScene":
                SceneManager.LoadScene("startScene", LoadSceneMode.Single);
                break;

            case "selectLocationScene":
                SceneManager.LoadScene("selectCharacterScene", LoadSceneMode.Single);
                break;

            case "previewScene":
                SceneManager.LoadScene("selectLocationScene", LoadSceneMode.Single);
                break;

            case "cameraScene":
                SceneManager.LoadScene("previewScene", LoadSceneMode.Single);
                break;
        }
    }
}
