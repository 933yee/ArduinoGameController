using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class threeDhuman : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator threeDhuman_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        threeDhuman_.SetTrigger("enter");
        state = true;
        // Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        threeDhuman_.SetTrigger("leave");
        state = false;
        // Debug.Log("LEAVE");
    }
}
