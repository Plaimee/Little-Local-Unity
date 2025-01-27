using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLocationScript : MonoBehaviour
{
    public static SelectLocationScript instance;
    public GameObject layoutLocation;
    public Button nextLocation;
    public Button previousLocation;
    public Button checkCurrent;
    public int firstLocation = 0;
    public int secLocation = 1;
    private int previousIndex = -1;

    public string currentLocationName;

    void Start()
    {
        instance = this;
        if(nextLocation != null)
        {
            nextLocation.onClick.AddListener(NextLocation);
            previousLocation.onClick.AddListener(PreviousLocation);
            checkCurrent.onClick.AddListener(CheckCurrentLocation);
        }
        else
        {
            Debug.LogError("Buttons are not assigned in the Inspector.");
        }
    }

    // Update is called once per frame
    void NextLocation()
    {
        if (layoutLocation != null)
        {
            Transform parentTransform = layoutLocation.transform;

            // Validate indices
            if (firstLocation >= 0 && firstLocation < parentTransform.childCount &&
                secLocation >= 0 && secLocation < parentTransform.childCount)
            {
                Transform child = parentTransform.GetChild(firstLocation);

                // Save the current index before moving
                previousIndex = child.GetSiblingIndex();

                // Move the child to the new position
                child.SetSiblingIndex(secLocation);
                Debug.Log($"Moved {child.gameObject.name} from position {firstLocation} to {secLocation}");
            }
            else
            {
                Debug.LogError($"Invalid index values. Valid range: 0 to {parentTransform.childCount - 1}");
            }
        }
        else
        {
            Debug.LogError("Horizontal Layout Group is not assigned.");
        }
    }

    void PreviousLocation()
    {
        if (layoutLocation != null)
        {
            Transform parentTransform = layoutLocation.transform;

            // Validate the stored previous index
            if (previousIndex >= 0 && previousIndex < parentTransform.childCount)
            {
                Transform child = parentTransform.GetChild(secLocation);

                // Move the child back to the previous index
                child.SetSiblingIndex(previousIndex);
                Debug.Log($"Moved {child.gameObject.name} back to position {previousIndex}");

                // Reset the previous index
                previousIndex = -1;
            }
            else
            {
                Debug.LogError("No valid previous position to move back to.");
            }
        }
        else
        {
            Debug.LogError("Horizontal Layout Group is not assigned.");
        }
    }

    void CheckCurrentLocation()
    {
        if (layoutLocation != null)
        {
            Transform parentTransform = layoutLocation.transform;
            if(parentTransform.childCount > 0)
            {
                Transform child = parentTransform.GetChild(0);
                currentLocationName = child.gameObject.name;
                Debug.Log(currentLocationName);
            }
            else
            {
                Debug.LogError("No children in the layout.");
            }
        }
        else
        {
            Debug.LogError("Horizontal Layout Group is not assigned.");
        }
    }
}
