using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private GameObject GunObject;
    private Vector3 mousePos;
    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
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

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            GunObject.GetComponent<Animator>().SetTrigger("Shoot");
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
        }
    }
}
