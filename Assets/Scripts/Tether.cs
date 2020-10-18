using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Tether : MonoBehaviour
{
    [SerializeField] Gradient healColor;
    [SerializeField] Gradient nuetralColor;
    [SerializeField] Gradient damageColor;

    private Transform candleTransform;
    private Transform jackTransform;
    private LineRenderer tether;

    private float healingDistance;
    private float damageDistance;
    private float currentDistance;


    private void Awake()
    {
        tether = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject jack = GameObject.FindWithTag("Player");
        jackTransform = jack.transform;
        healingDistance = jack.GetComponent<PlayerStatus>().healingDistanceFromCandle;
        damageDistance = jack.GetComponent<PlayerStatus>().damagingDistanceFromCandle;


        candleTransform = GameObject.FindWithTag("Candle").transform;

        tether.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        tether.SetPosition(0, candleTransform.position);
        tether.SetPosition(1, jackTransform.position);

        currentDistance = Vector2.Distance(candleTransform.position, jackTransform.position);

        if(currentDistance < healingDistance)
        {
            tether.colorGradient = healColor;
        }
        else if (currentDistance < damageDistance)
        {
            tether.colorGradient = nuetralColor;
        }
        else
        {
            tether.colorGradient = damageColor;
        }
    }
}
