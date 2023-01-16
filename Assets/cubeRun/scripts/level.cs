using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level : MonoBehaviour
{
    public Animator level_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        level_.SetTrigger("enter");
        state = true;
        Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        level_.SetTrigger("leave");
        state = false;
        Debug.Log("LEAVE");
    }
}
