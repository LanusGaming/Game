using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool interactable;

    public Action<Trigger> triggeredCallback;
    public Action<Trigger> interactionEnteredCallback;
    public Action<Trigger> interactionActiveCallback;
    public Action<Trigger> interactionExitedCallback;

    private Coroutine waitingRoutine;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
            return;

        if (interactable)
        {
            interactionEnteredCallback.Invoke(this);
            waitingRoutine = StartCoroutine(WaitForInput());
        }
        else
        {
            triggeredCallback.Invoke(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && interactable)
        {
            StopCoroutine(waitingRoutine);
            interactionExitedCallback.Invoke(this);
        }
    }

    private IEnumerator WaitForInput()
    {
        while (!InputManager.InteractPressed)
        {
            interactionActiveCallback.Invoke(this);
            yield return new WaitForEndOfFrame();
        }

        triggeredCallback.Invoke(this);
    }
}
