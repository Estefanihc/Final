using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun1 : MonoBehaviour
{


    public bool canFire;
    public float FireRate = .5f;

    [SerializeField] private GameObject GunObject;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    private float timer;
    private Camera mainCam;
    private Vector3 mousePos;
    private Rigidbody2D playerRb;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);


        timer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && timer <= 0)
        {
            GunObject.GetComponent<Animator>().SetTrigger("Shoot");
            Shot();
            timer = FireRate;


        }

        flipGun();
    }
    void Shot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    }

    void flipGun()
    {
        float zRotation = transform.eulerAngles.z;

        // Normalize the rotation angle to the range of -180 to 180 degrees
        if (zRotation > 180)
        {
            zRotation -= 360;
        }

        // Check the normalized rotation angle to determine the sprite flip
        if (zRotation <= 90 && zRotation >= -90)
        {
            GunObject.GetComponent<SpriteRenderer>().flipY = false;
        }
        else
        {
            GunObject.GetComponent<SpriteRenderer>().flipY = true;
        }
    }
}
