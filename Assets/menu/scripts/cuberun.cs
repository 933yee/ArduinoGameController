using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cuberun : MonoBehaviour
{
    public Animator cuberun_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        cuberun_.SetTrigger("enter");
        state = true;
        // Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        cuberun_.SetTrigger("leave");
        state = false;
        // Debug.Log("LEAVE");
    }
}
