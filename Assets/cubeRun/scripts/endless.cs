using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endless : MonoBehaviour
{
    public Animator endless_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        endless_.SetTrigger("enter");
        state = true;
        Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        endless_.SetTrigger("leave");
        state = false;
        Debug.Log("LEAVE");
    }
}
