using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] float maxFollowDistanceFromCandle = 9;
    [SerializeField] float jackCameraOffset = 3;
    [SerializeField] float maxZoomDistance = 12;
    [Range(0.5f, 10f)]
    [SerializeField] float cameraMinSpeed = 1f;
    [Range(1f, 10f)]
    [SerializeField] float cameraMaxSpeed = 4f;
    [SerializeField] float maxCameraDistance = 6;
    [SerializeField] float minZoom = 4f;
    [SerializeField] float maxZoom = 6f;

    private Transform jack;
    private Transform candle;

    private Vector3 velocity;

    private Camera cam;

    private float convertedMinSpeed;
    private float convertedMaxSpeed;

    private void Start()
    {
        jack = GameObject.FindWithTag("Player").transform;
        candle = GameObject.FindWithTag("Candle").transform;

        cam = GetComponent<Camera>();

        if (offset == null)
        {
            offset = transform.position;
        }
        else
        {
            offset += transform.position;
        }

        if (maxCameraDistance < jackCameraOffset)
        {
            Debug.LogWarning("maxCameraDistance must be LARGER than jackCameraOffset");
            maxCameraDistance = jackCameraOffset;
        }

        convertedMinSpeed = 1 / cameraMinSpeed;
        convertedMaxSpeed = 1 / cameraMaxSpeed;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        Vector2 centerPoint = GetMovePosition();

        Vector3 normalizedNewPosition = new Vector3(centerPoint.x + offset.x, centerPoint.y + offset.y, offset.z);

        transform.position = Vector3.SmoothDamp(transform.position, normalizedNewPosition, ref velocity, CameraSpeed());
    }

    float CameraSpeed()
    {
        var retval = cameraMinSpeed;
        var cameraDistanceFromJack = Vector3.Distance(transform.position, jack.position);
        //only adjust away from min speed if we are greater than the offset away
        if (cameraDistanceFromJack > jackCameraOffset)
        {
            //if its greater than the max camera distance we want to use the max speed val
            if (cameraDistanceFromJack < maxCameraDistance)
            {
                retval = cameraMaxSpeed;
            }
            //this value is between the jackCameraOffset and the maxCameraDistance
            else
            {
            var adjustedDistance = cameraDistanceFromJack - jackCameraOffset;
            var adjustedMaxDistance = maxCameraDistance - jackCameraOffset;

            retval = Mathf.Lerp(convertedMinSpeed, convertedMaxSpeed, adjustedDistance / adjustedMaxDistance);
            }
        }
        return retval;
    }

    void ZoomCamera()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetDistance() / maxZoomDistance);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    private Vector2 GetMovePosition()
    {
        var bounds = new Bounds(jack.position, Vector3.zero);
        bounds.Encapsulate(candle.position);

        var center = bounds.center;

        var focalPoint = Vector3.MoveTowards(jack.position, center, jackCameraOffset);
        return focalPoint;
    }

    private float GetDistance()
    {
        return Vector2.Distance(jack.position, candle.position);
    }
}
