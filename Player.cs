using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using static PlayerControls;
using Cinemachine;
using static Settings;
using static HelperScripts;


public class Player : MonoBehaviour
{

    private bool freeFalling = false;
    public Vector3 test1;
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        if (freeFalling) return velocity;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {

            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

            Vector3 adjustedVelocity = slopeRotation * velocity;
            if (adjustedVelocity.y > -0.75) { return adjustedVelocity; }
            else { freeFalling = true; }

        }

        return velocity;
    }

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject cameraManager;

    private CamSwitcher camSwitcher;

    private PlayerControls playerControls;

    private Vector3 moveDirection;

    [SerializeField] private CharacterController controller;

    public float playerSpeed = 2.0f;

    private Vector2 mousePointer;

    [SerializeField] Camera mainCamera;

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        controller.slopeLimit = 120;
        camSwitcher = cameraManager.GetComponent<CamSwitcher>();

    }
    private float zoomLevel = 5;
    private Vector2 camZoom;
    private void HandleSettingsGo()
    {
        camZoom.Normalize();
        if (camSwitcher.tepegozCam)
        {

            CinemachineFramingTransposer framingTransposer = camSwitcher.getVcam1().GetCinemachineComponent<CinemachineFramingTransposer>();
            if (camZoom.y > 0 && framingTransposer.m_CameraDistance >= 20)
            {
                framingTransposer.m_CameraDistance -= camZoom.y * 10;
                zoomLevel += 1;
                framingTransposer.m_XDamping -= 0.18f;
                framingTransposer.m_YDamping -= 0.14f;
                camZoom.y = 0;
            }
            else if (camZoom.y < 0 && framingTransposer.m_CameraDistance <= 130)
            {
                framingTransposer.m_CameraDistance -= camZoom.y * 10;
                zoomLevel--;
                framingTransposer.m_XDamping += 0.18f;
                framingTransposer.m_YDamping += 0.14f;
                camZoom.y = 0;
            }



        }
        else
        {
            CinemachineCameraOffset cameraOffset = camSwitcher.getVcam2().GetComponent<CinemachineCameraOffset>();
            if (camZoom.y > 0 && cameraOffset.m_Offset.y > 3)
            {
                cameraOffset.m_Offset.y -= 1;
                cameraOffset.m_Offset.z += 2f;

                camZoom.y = 0;
            }
            if (camZoom.y < 0 && cameraOffset.m_Offset.y < 10)
            {
                cameraOffset.m_Offset.y += 1;
                cameraOffset.m_Offset.z -= 2f;
                camZoom.y = 0;
            }

        }
    }


    private void HandleMovement()
    {

        if (camSwitcher.tepegozCam && (playerControls.Movement.Run.WasPerformedThisFrame() || playerControls.Movement.Run.IsPressed()))
        {
            Terrain activeTerrain = HelperScripts.GetClosestCurrentTerrain(transform.position);
            float yAddOn = transform.position.y - activeTerrain.SampleHeight(transform.position);
            moveDirection = new Vector3(movementInput.x, 0.0f, movementInput.y);
            moveDirection.Normalize();

            //  moveDirection.y = 0;
            float angle = Vector3.Angle(Vector3.forward, transform.forward);
            Vector3 cross = Vector3.Cross(Vector3.forward, transform.forward);
            if (cross.y < 0) angle = -angle;
            moveDirection = Quaternion.Euler(0, angle, 0) * moveDirection;
            moveDirection = AdjustVelocityToSlope(moveDirection);
            moveDirection.Normalize();
            moveDirection *= ((-1.0f) * moveDirection.y * (0.1f) + 1);
            controller.Move(moveDirection * Time.deltaTime * playerSpeed);

            // controller.Move(new Vector3(transform.position.x, activeTerrain.SampleHeight(transform.position) + yAddOn, transform.position.z) - transform.position);
        }
        else if (camSwitcher.tepegozCam && playerControls.Movement.Run.WasReleasedThisFrame()) { movementInput = new Vector2(0, 0); controller.Move(moveDirection * Time.deltaTime * playerSpeed); }
        //bir tane daha gidiyor
        else if (!camSwitcher.tepegozCam && (playerControls.Movement.Run.WasPerformedThisFrame() || playerControls.Movement.Run.IsPressed()))
        {
            Terrain activeTerrain = HelperScripts.GetClosestCurrentTerrain(transform.position);
            float yAddOn = transform.position.y - activeTerrain.SampleHeight(transform.position);
            moveDirection = new Vector3(movementInput.x, 0.0f, movementInput.y);

            moveDirection.Normalize();
            //  moveDirection.y = 0;
            Vector3 mainCameraForwardVector = mainCamera.transform.forward;
            mainCameraForwardVector.y = 0;
            float angle = Vector3.Angle(Vector3.forward, mainCameraForwardVector);
            Vector3 cross = Vector3.Cross(Vector3.forward, mainCameraForwardVector);
            if (cross.y < 0) angle = -angle;
            moveDirection = Quaternion.Euler(0, angle, 0) * moveDirection;
            transform.forward = moveDirection;
            moveDirection = AdjustVelocityToSlope(moveDirection);
            moveDirection.Normalize();
            moveDirection *= ((-1.0f) * moveDirection.y * (0.1f) + 1);

            controller.Move(moveDirection * Time.deltaTime * playerSpeed);
            // controller.Move(new Vector3(transform.position.x, activeTerrain.SampleHeight(transform.position) + yAddOn, transform.position.z) - transform.position);
        }
        playerVelocity.y += gravityValue * Time.deltaTime * (Time.deltaTime + 1);
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private float oldCameraYAngle = 0;
    private Vector3 lookDirection;
    private float lookLength;
    private Vector2 mousePosition2d;
    private float camRotateAngle;



    private Terrain activeTerrain;
    private void HandleDash()
    {
        if (camSwitcher.tepegozCam && dashedThisFrame)
        {
            Vector3 dashAimPoint = HelperScripts.MouseToWorld() - transform.position;
            Vector2 dashAimPoint2d = new Vector2(dashAimPoint.x, dashAimPoint.z);
            if (dashAimPoint2d.magnitude > 30)
            {
                dashAimPoint2d.Normalize();
                dashAimPoint2d = dashAimPoint2d * 30;
                dashAimPoint.x = dashAimPoint2d.x;
                dashAimPoint.z = dashAimPoint2d.y;
            }
            activeTerrain = HelperScripts.GetClosestCurrentTerrain(dashAimPoint + transform.position);
            dashAimPoint.y = activeTerrain.SampleHeight(dashAimPoint + transform.position) - transform.position.y;
            //   dashAimPoint.y -= activeTerrain.SampleHeight(dashAimPoint + transform.position);
            controller.Move(dashAimPoint);
        }
        else if (!camSwitcher.tepegozCam && dashedThisFrame)
        {
            Vector3 mainCameraForwardVector = mainCamera.transform.forward;
            mainCameraForwardVector.y = 0;
            Vector3 dashAimPoint = mainCameraForwardVector;
            dashAimPoint.Normalize();
            dashAimPoint *= 30;
            activeTerrain = HelperScripts.GetClosestCurrentTerrain(dashAimPoint + transform.position);
            dashAimPoint.y = activeTerrain.SampleHeight(dashAimPoint + transform.position) - transform.position.y;
            //   dashAimPoint.y -= activeTerrain.SampleHeight(dashAimPoint + transform.position);
            controller.Move(dashAimPoint);

        }
        dashedThisFrame = false;
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.identity;
        if (camSwitcher.tepegozCam)
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))

            {
                CinemachineFramingTransposer framingTransposer = camSwitcher.getVcam1().GetCinemachineComponent<CinemachineFramingTransposer>();


                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                lookDirection = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);
                transform.LookAt(lookDirection);
                if (playerControls.SettingsGo.CamRotate.IsPressed())
                {
                    framingTransposer.m_TrackedObjectOffset.z = 0;
                    mousePosition2d = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);

                    camRotateAngle = Vector2.SignedAngle(new Vector2(camRotationMouseStartPos.x, camRotationMouseStartPos.y), mousePosition2d);

                    camSwitcher.getVcam1().transform.rotation = Quaternion.Euler(90, oldCameraYAngle - camRotateAngle, 0);


                }
                else if (playerControls.SettingsGo.CamRotate.WasReleasedThisFrame())
                {
                    oldCameraYAngle = oldCameraYAngle - camRotateAngle;

                    lookLength = (new Vector2(lookDirection.x - transform.position.x, lookDirection.z - transform.position.z)).magnitude;

                    framingTransposer.m_TrackedObjectOffset.z = lookLength / 12 * ((12 - zoomLevel) / 12);
                }
                else
                {
                    lookLength = (new Vector2(lookDirection.x - transform.position.x, lookDirection.z - transform.position.z)).magnitude;

                    framingTransposer.m_TrackedObjectOffset.z = lookLength / 12 * ((12 - zoomLevel) / 12);
                }


            }
        }
        else
        {
            Quaternion thirdRotation = mainCamera.transform.rotation;
            thirdRotation.z = 0;
            thirdRotation.x = 0;
            transform.rotation = thirdRotation;
            //   mainCamera.transform.rotation = Quaternion.identity;
        }
    }

    private Vector2 movementInput;

    private bool dashedThisFrame = false;
    private Vector2 camRotationMouseStartPos;
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Movement.Run.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.Movement.Jump.performed += _ => playerVelocity.y = (playerVelocity.y - gravityValue * Time.deltaTime + 22.0f);
            playerControls.Movement.MousePosition.performed += i => mousePointer = i.ReadValue<Vector2>();
            playerControls.SettingsGo.CamZoom.performed += i => camZoom = i.ReadValue<Vector2>().normalized;
            playerControls.SettingsGo.CamRotate.performed += i => camRotationMouseStartPos = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
            playerControls.Movement.Dash.performed += _ => dashedThisFrame = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private float gravityValue = -39.81f;
    private bool groundedPlayer;
    private Vector3 playerVelocity;
    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (!groundedPlayer)
        {
            playerControls.Movement.Jump.Disable();
        }
        if (groundedPlayer)
        {
            playerControls.Movement.Jump.Enable();
            freeFalling = false;
        }
        HandleRotation();
        HandleMovement();
        HandleSettingsGo();
        HandleDash();



    }




}
