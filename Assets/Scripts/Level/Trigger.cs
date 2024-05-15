using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Action<Trigger> callback;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.tag);

        if (collider.gameObject.tag == "Player" && callback is not null)
        {
            callback.Invoke(this);
        }
    }
}
