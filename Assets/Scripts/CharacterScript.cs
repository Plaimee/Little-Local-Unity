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

                    // Handle the "random" button
                    if (btnName == "Random")
                    {
                        SelectRandomButton();
                    }
                    else
                    {
                        nextBtn.gameObject.SetActive(true);
                        isSelected = true;
                    }
                    break;
                }
            }
            previousSelected = selectedLittle;
        }
        else if (selectedLittle == null)
        {
            nextBtn.gameObject.SetActive(false);
            previousSelected = null;
        }
    }

    // Method to randomly select a button except "Random"
    void SelectRandomButton()
    {
        List<Button> validButtons = new List<Button>();

        // Exclude the "random" button
        foreach (Button btn in little)
        {
            if (btn.gameObject.name != "Random")
            {
                validButtons.Add(btn);
            }
        }

        if (validButtons.Count > 0)
        {
            int randomIndex = Random.Range(0, validButtons.Count); // Randomly pick an index
            Button randomButton = validButtons[randomIndex]; // Get the button
            randomButton.Select(); // Highlight the button
            Debug.Log(randomButton.gameObject.name + " is randomly selected");

            // Set up the state
            btnName = randomButton.gameObject.name;
            nextBtn.gameObject.SetActive(true);
            isSelected = true;
        }
    }
}
