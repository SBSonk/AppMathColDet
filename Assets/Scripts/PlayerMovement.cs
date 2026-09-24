using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5;

    private void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(  xInput, 0, yInput).normalized;
        transform.position += input * moveSpeed * Time.deltaTime;
    }
}
