using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCtrlScript : MonoBehaviour
{
    public GameObject ggGroup;
    public GameObject[] ggHead1;
    public GameObject[] ggHead2;
    public GameObject[] pdEyes1;
    public GameObject[] pdEyes2;
    public GameObject[] ggMouth1;
    public GameObject[] ggMouth2;

    void Start()
    {
        ActivateRandomGameObject();
    }

    public void ActivateRandomGameObject()
    {
        if (CharacterScript.instance.btnName == "GG")
        {
            DisableAllObjects(pdEyes1);
            DisableAllObjects(pdEyes2);
            DisableAllObjects(ggHead1);
            DisableAllObjects(ggHead2);
            DisableAllObjects(ggMouth1);
            DisableAllObjects(ggMouth2);

            ActivateRandomFromArray(ggHead1);
            ActivateRandomFromArray(ggHead2);
            ActivateRandomFromArray(ggMouth1);
            ActivateRandomFromArray(ggMouth2);
        }
        
    }

    private void DisableAllObjects(GameObject[] objects)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void ActivateRandomFromArray(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return;

        int randomIndex = Random.Range(0, objects.Length);

        if (objects[randomIndex] != null)
        {
            objects[randomIndex].SetActive(true);
            Debug.Log("Activated: " + objects[randomIndex].name);
        }
    }

    public GameObject GetActiveMouth()
    {
        return FindActiveObject(ggMouth1) ?? FindActiveObject(ggMouth2);
    }

    private GameObject FindActiveObject(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.activeSelf)
            {
                return obj;
            }
        }
        return null;
    }
}
