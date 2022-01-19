using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public bool throttle => Input.GetKey(KeyCode.Space);

    public float pitchPower, rollPower, yawPower, enginePower;
    private float activeRoll, activePitch, activeYaw;

    public CinemachineDollyCart dollyCart;

    public float moveSpeed = 2f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        if(throttle)
        {
            dollyCart.m_Speed = 1f;
        } else
        {
            dollyCart.m_Speed = 0.5f;
        }

        Vector3 moveInput = new Vector3(0, Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        Move(moveVelocity);

        //activePitch = Input.GetAxisRaw("Vertical") * pitchPower * Time.deltaTime;
        //activeRoll = Input.GetAxisRaw("Horizontal") * rollPower * Time.deltaTime;
        //activeYaw = Input.GetAxisRaw("Fire3") * yawPower * Time.deltaTime;

        //transform.Rotate(activePitch * pitchPower * Time.deltaTime, 
        //    activeYaw * yawPower * Time.deltaTime,
        //    -activeRoll * rollPower * Time.deltaTime,
        //    Space.Self);

        //transform.Translate(Mathf.Clamp(-VelocidadeMov, -5, 6), 0, 0);

    }

    public void Move(Vector3 _velocity)
    {
        rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
    }


}
