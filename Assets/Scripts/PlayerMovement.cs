using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5;

    private void Update()
    {
        float xIn = Input.GetAxisRaw("Horizontal");
        float yIn = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(xIn) > 0 && Mathf.Abs(yIn) < .25f) 
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime * xIn;
            return;
        }

        if (Mathf.Abs(yIn) > 0) 
        {
            transform.position += Vector3.forward * moveSpeed * Time.deltaTime * yIn;
            return;
        }
    }
}
