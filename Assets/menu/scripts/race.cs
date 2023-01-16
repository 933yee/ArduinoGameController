using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class race : MonoBehaviour
{
    public Animator race_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        race_.SetTrigger("enter");
        state = true;
        // Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        race_.SetTrigger("leave");
        state = false;
        // Debug.Log("LEAVE");
    }
}
