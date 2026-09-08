using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5;

    private void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        transform.position += input * moveSpeed * Time.deltaTime;
    }
}
