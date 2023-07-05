using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperScripts;
public class PlayerAimScript : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] Projector aim;
    [SerializeField] GameObject cameraManager;
    private CamSwitcher camSwitcher;
    [SerializeField] Camera mainCamera;
    private PlayerControls playerControls;

    [SerializeField] Canvas crosshair;

    private float lastFired;
    private float fireRate = 10;
    void Start()
    {
        camSwitcher = cameraManager.GetComponent<CamSwitcher>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Combat.Shoot.performed += _ => ShootEvent();


        }

        playerControls.Enable();
    }

    [SerializeField] private Transform bullet;
    private void ShootEvent()
    {
        Instantiate(bullet, CalculateWeaponEndPoint(), weapon.transform.rotation);
        lastFired = Time.time;

    }

    private void ShootManager()
    {

        if (Time.time - lastFired >= 1f / fireRate)
        {
          
            ShootEvent();
        }
    }

    public Vector3 test1;
    private Vector3 CalculateWeaponEndPoint()
    {
        Vector3 weaponEndPoint = weapon.transform.position;
        weaponEndPoint += weapon.transform.forward * (weapon.GetComponent<Renderer>().bounds.size.z / 2);
        test1 = weaponEndPoint;
        return weaponEndPoint;
    }


    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (camSwitcher.tepegozCam)
        {
            Vector3 pointer = aim.transform.position;
            pointer.y -= 9.80f;
            weapon.transform.LookAt(pointer, Vector3.up);
        }
        else if (!camSwitcher.tepegozCam)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 500f))
            {
                weapon.transform.LookAt(hit.point);

            }
            else { weapon.transform.LookAt(ray.GetPoint(200f)); }
        }
        if (playerControls.Combat.Shoot.IsPressed())
        {
            ShootManager();
        }

    }
}

