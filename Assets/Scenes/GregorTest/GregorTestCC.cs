using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GregorTestCC : MonoBehaviour
{
    [SerializeField]
    public Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.AddForce(Vector2.right * Time.deltaTime * 200.0f);
    }
}
