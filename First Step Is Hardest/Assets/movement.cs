using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 200f;
    public float sprintSpeed = 10f;
    private float currentSpeed;

    private bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, 0, moveZ));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}