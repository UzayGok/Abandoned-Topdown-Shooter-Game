


using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Cinemachine;



public class CamSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction action;
    [SerializeField] public bool tepegozCam;
    [SerializeField] private CinemachineVirtualCamera vcam1; //tepegoz
    [SerializeField] private CinemachineFreeLook vcam2; //3rdperson
    [SerializeField] private GameObject tepegozAim;
    [SerializeField] private GameObject crosshair;
    private ButtonControl buttonControl;
    private CinemachineBrain brain;

    private float vcam2xMaxSpeed;
    private float vcam2yMaxSpeed;

    private Transform vcam2LookAt;
    private void OnEnable()
    {
        action.Enable();
    }
    private void OnDisable()
    {
        action.Disable();
    }
    void Start()
    {
        brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcam2);
        vcam2xMaxSpeed = vcam2.m_XAxis.m_MaxSpeed;
        vcam2yMaxSpeed = vcam2.m_YAxis.m_MaxSpeed;
        vcam2LookAt = vcam2.m_LookAt;
        tepegozCam = true;

    }
    private void Awake()
    {
        buttonControl = (ButtonControl)action.controls[0];
        action.Enable();
    }

    public CinemachineVirtualCamera getVcam1() { return vcam1; }

    public CinemachineFreeLook getVcam2() { return vcam2; }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (buttonControl.wasPressedThisFrame)
        {
            vcam2.Priority = 2;
            tepegozCam = false;
            vcam2.m_LookAt = vcam2LookAt;
            tepegozAim.SetActive(false);
            crosshair.SetActive(true);
        }
        if (buttonControl.wasReleasedThisFrame)
        {
            vcam2.Priority = 0;
            tepegozCam = true;
            vcam2.m_LookAt = null;
            tepegozAim.SetActive(true);
            crosshair.SetActive(false);
        }
        if (brain.IsBlending)
        {
            vcam2.m_XAxis.m_MaxSpeed = 0;
            vcam2.m_YAxis.m_MaxSpeed = 0;

        }
        else
        {
            vcam2.m_XAxis.m_MaxSpeed = vcam2xMaxSpeed;
            vcam2.m_YAxis.m_MaxSpeed = vcam2yMaxSpeed;

        }


    }
}
