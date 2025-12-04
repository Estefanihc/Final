using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
            
        }
    }


    public void Open() {
    GetComponent<Animator>().SetBool("Open",true);
    }

    public void Close() {
        GetComponent<Animator>().SetBool("Open", false);
    }
}
