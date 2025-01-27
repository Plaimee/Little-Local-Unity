using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    public static CharacterScript instance;
    public Button nextBtn;
    public Button[] little;
    public string btnName;
    GameObject previousSelected;
    public bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        nextBtn.gameObject.SetActive(false);
    }

    void Update()
    {
        GameObject selectedLittle = EventSystem.current.currentSelectedGameObject;

        if (selectedLittle != null && selectedLittle != previousSelected)
        {
            isSelected = false;
            for (int i = 0; i < little.Length; i++)
            {
                if (little[i].gameObject == selectedLittle)
                {
                    btnName = little[i].gameObject.name;
                    Debug.Log(btnName + " is selected");
                    nextBtn.gameObject.SetActive(true);
                    isSelected = true;
                    break;
                }
            }
            previousSelected = selectedLittle;
        } else if (selectedLittle == null)
        {
            nextBtn.gameObject.SetActive(false);
            previousSelected = null;
        }
    }
}
