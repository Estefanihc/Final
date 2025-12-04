using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Vector3 m_Position;
    void Start()
    {
        transform.position = m_Position;
    }

    private void Update()
    {
        if(transform.parent != null)
        transform.position = transform.parent.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().enabled = false;
            transform.parent = collision.transform;
        }
        if (collision.tag=="Gate") {
        
            collision.GetComponent<Gate>().Open();
        Destroy(gameObject);
        
        }
    }
}
