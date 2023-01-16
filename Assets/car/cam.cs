using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    public Transform player;
    float previousY = 80f;
    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + player.forward * -7f + new Vector3(0, 5f, 0);
        if (Mathf.Abs(previousY - player.rotation.y) <= 0.5f)
            transform.rotation = player.rotation * Quaternion.Euler(35, previousY, 0);
        else
            transform.rotation = player.rotation * Quaternion.Euler(35, 0, 0);
        previousY = transform.rotation.y;
    }
}
