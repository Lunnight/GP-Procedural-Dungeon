using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject endScreen;

    // variables
    bool isGrounded;
    public float jumpForce;
    public int moveSpeed;
    Rigidbody rb;

    int multiplyX;
    int multiplyZ;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = true;
    }

    void Update()
    {
        //so that 'w,a,s,d' or arrow keys still make the player move correctly no matter where they're facing
        switch (transform.rotation.eulerAngles.y)
        {
            case 0:
                multiplyX = 1;
                multiplyZ = 0;
                break;
            case 90:
                multiplyX = 0;
                multiplyZ = 1;
                break;
            case 180:
                multiplyX = -1;
                multiplyZ = 0;
                break;
            case 270:
                multiplyX = 0;
                multiplyZ = -1;
                break;
        }

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            multiplyZ *= -1;    //otherwise left and right movement will be inverted when facing z directions
            rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * multiplyX, rb.velocity.y, Input.GetAxisRaw("Horizontal") * moveSpeed * multiplyZ);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            rb.velocity = new Vector3(Input.GetAxisRaw("Vertical") * moveSpeed * multiplyZ, rb.velocity.y, Input.GetAxisRaw("Vertical") * moveSpeed * multiplyX);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            rb.AddForce(new Vector3(rb.velocity.x, jumpForce, rb.velocity.z), ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -90, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 90, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Finish")
        {
            endScreen.SetActive(true);
        }
    }
}
