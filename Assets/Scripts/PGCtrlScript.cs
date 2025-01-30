using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGCtrlScript : MonoBehaviour
{
    public GameObject pgGroup;
    public GameObject[] pdHead;
    public GameObject[] eyes1;
    public GameObject[] pdMouth;

    public GameObject[] gtwHead;
    public GameObject[] eyes2;
    public GameObject[] gtwMouth;

    void Start()
    {
        ActivateRandomGameObject();
    }

    public void ActivateRandomGameObject()
    {
        if (CharacterScript.instance.btnName == "PG")
        {
            DisableAllObjects(pdHead);
            DisableAllObjects(eyes1);
            DisableAllObjects(pdMouth);
            DisableAllObjects(gtwHead);
            DisableAllObjects(eyes2);
            DisableAllObjects(gtwMouth);

            ActivateRandomFromArray(pdHead);
            ActivateRandomFromArray(pdMouth);
            ActivateRandomFromArray(gtwHead);
            ActivateRandomFromArray(gtwMouth);
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
        }
    }

    public GameObject GetActiveMouth()
    {
        return FindActiveObject(pdMouth) ?? FindActiveObject(gtwMouth);
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
