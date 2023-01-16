using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tetris : MonoBehaviour
{
    public Animator tetris_;
    public bool state = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        tetris_.SetTrigger("enter");
        state = true;
        // Debug.Log("ENTER");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        tetris_.SetTrigger("leave");
        state = false;
        // Debug.Log("LEAVE");
    }
}
