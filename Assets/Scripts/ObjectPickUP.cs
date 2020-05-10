using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickUP : MonoBehaviour
{

    [SerializeField] float posX;
    [SerializeField] float posY;
    [SerializeField] float posZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.position = new Vector3(posX,posY, posZ);
        }
    }*/

    private void OnMouseDown()
    {
        if(Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position) < 1);
        {
            transform.position = new Vector3(posX,posY, posZ);    
        }
        
    }
}
