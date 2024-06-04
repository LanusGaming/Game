using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Game;

public class Control : MenuItem<Settings.Controls.Control>
{
    public GameObject keySelectorOverlay;
    public TextMeshProUGUI controlNameText;
    public TextMeshProUGUI primaryKeyText;
    public TextMeshProUGUI secondaryKeyText;
    public GameObject resetPrimaryKeyButton;
    public GameObject resetSecondaryKeyButton;

    public override void SetValue(Settings.Controls.Control value)
    {
        base.SetValue(value);

        controlNameText.text = value.name;
        primaryKeyText.text = value.primaryKey.ToString();
        secondaryKeyText.text = value.secondaryKey.ToString();

        Settings.Controls.Control defaulControl = (new Settings.Controls()).GetControlByName(value.name);

        if (defaulControl.primaryKey != value.primaryKey)
            resetPrimaryKeyButton.SetActive(true);
        else
            resetPrimaryKeyButton.SetActive(false);

        if (defaulControl.secondaryKey != value.secondaryKey)
            resetSecondaryKeyButton.SetActive(true);
        else
            resetSecondaryKeyButton.SetActive(false);
    }

    public void ResetKey(bool primary)
    {
        Settings.Controls.Control defaulControl = (new Settings.Controls()).GetControlByName(value.name);

        if (primary)
            value.primaryKey = defaulControl.primaryKey;
        else
            value.secondaryKey = defaulControl.secondaryKey;

        SetValue(value);
    }

    public void ChangeKey(bool primary)
    {
        StartCoroutine(WaitForKey(primary));
    }

    private IEnumerator WaitForKey(bool primary)
    {
        InputManager.Focus("awaitKey");
        keySelectorOverlay.SetActive(true);

        yield return new WaitUntil(() => InputManager.ActiveKeys.Length > 0);

        if (primary)
            value.primaryKey = InputManager.ActiveKeys[0];
        else
            value.secondaryKey = InputManager.ActiveKeys[0];

        SetValue(value);

        keySelectorOverlay.SetActive(false);
        InputManager.Unfocus();
    }
}