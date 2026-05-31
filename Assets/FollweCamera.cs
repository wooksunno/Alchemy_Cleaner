using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cam;

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = cam.position - transform.position;
        dir.y = 0;
        transform.forward = -dir;
    }
}
