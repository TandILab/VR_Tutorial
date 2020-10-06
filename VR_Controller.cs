using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VR_Controller : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float sensitivity = 0.1f;
    public float gravity = 30.0f;

    public SteamVR_Action_Boolean movePress = null;
    public SteamVR_Action_Vector2 moveValue = null;

    private float speed = 1.0f;

    private CharacterController controller = null;
    private Transform cameraRig = null;
    private Transform head = null;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cameraRig = SteamVR_Render.Top().origin;
        head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
        HandleHead();
        HandleHeight();
        CalculateMovement();

    }

    private void HandleHead()
    {
        //Store Current
        Vector3 oldPosition = cameraRig.position;
        Quaternion oldRotation = cameraRig.rotation;

        //Rotation
        transform.eulerAngles = new Vector3(0.0f, head.rotation.eulerAngles.y, 0.0f);

        //Restore
        cameraRig.position = oldPosition;
        cameraRig.rotation = oldRotation;

    }

    private void CalculateMovement()
    {
        Vector3 orientationEuler = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        if (movePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }

        if (movePress.state)
        {
            speed += (moveValue.axis.y) * (sensitivity);
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

            movement += (orientation) * (speed * Vector3.forward);
        }

        movement.y -= gravity * Time.deltaTime;

        controller.Move(movement * Time.deltaTime);

    }

    private void HandleHeight()
    {
        float headHeight = Mathf.Clamp(head.localPosition.y, 1.5f, 3f);
        controller.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = controller.height / 2.5f;
        newCenter.y += controller.skinWidth;

        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        controller.center = newCenter;

    }
}
