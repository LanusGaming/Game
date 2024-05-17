using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Canvas canvas;

    public static UIController instance;

    private void Start()
    {
        instance = this;
    }

    public GameObject DisplayDialog(GameObject interactDialog, Vector2 position)
    {
        GameObject interactDialogObject = Instantiate(interactDialog, canvas.transform);
        interactDialogObject.transform.localPosition = Camera.main.WorldToScreenPoint(position);

        return interactDialogObject;
    }
}
