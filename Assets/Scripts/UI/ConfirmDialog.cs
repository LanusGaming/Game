using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmDialog : MonoBehaviour
{
    public string text
    {
        get { return textField.text; }
        set { textField.text = value; }
    }

    public TextMeshProUGUI textField;

    [HideInInspector]
    public Action<ConfirmDialog> confirm;

    public void OnConfirmButtonClick()
    {
        confirm.Invoke(this);
        Destroy(gameObject);
    }

    public void OnCancelButtonClick()
    {
        Destroy(gameObject);
    }
}