using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CandlePosition : MonoBehaviour
{

    public Vector2 candlePos;

    // Start is called before the first frame update
    void Start()
    {
        candlePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
