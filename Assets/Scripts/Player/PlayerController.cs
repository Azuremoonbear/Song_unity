using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public float Health = 100f;
    public float Damage = 10f;
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float jumpPower = 3f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);
        controller.Move(move * speed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
