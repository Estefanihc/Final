using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Axe : MonoBehaviour
{
    Camera mainCam;
    private Vector3 mousePos;
    [SerializeField] private GameObject AxeObject;
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
        flipGun();

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
            AxeObject.GetComponent<SpriteRenderer>().flipY = false;
        }
        else
        {
            AxeObject.GetComponent<SpriteRenderer>().flipY = true;
        }
    }
}