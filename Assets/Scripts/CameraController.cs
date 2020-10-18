using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform camTarget;
    public Vector3 camOffset;

    private void LateUpdate()
    {
        //transform.position = new Vector3(camTarget.position.x, camTarget.position.y, camTarget.position.z) + camOffset;
        transform.position = camTarget.position + camOffset;
    }
}
