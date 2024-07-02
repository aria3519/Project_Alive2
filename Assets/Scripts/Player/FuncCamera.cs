using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    [SerializeField] float zoomSpeed = 0f;
    [SerializeField] float valueSpeed = 0f;
    [SerializeField] float zoomMax = 0f;
    [SerializeField] float zoomMin = 0f;
    [SerializeField] float camSize = 0f;

    [SerializeField] float panSpeed = 20f;
    [SerializeField] float panBorderThickenss = 10f;
    [SerializeField] Vector2 panLimit;
    Vector3 forward, right;
    private void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);

        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }
    private void CameraZoom()
    {
        if (!Input.GetButton("Formation"))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                zoomSpeed = valueSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                zoomSpeed = -valueSpeed;
            }
        }
        if(zoomSpeed != 0)
        {
            zoomSpeed = zoomSpeed * 0.9f;

            if (Mathf.Abs(zoomSpeed) < 0.001)
                zoomSpeed = 0;
        }

        camSize = camSize - zoomSpeed;
        camSize = Mathf.Clamp(camSize, zoomMin, zoomMax);
        Camera.main.orthographicSize = camSize;
    }

    public void TargetSet(Transform changedTarget)
    {
        target = changedTarget;
    }

    
    private void CameraFollow()
    {
        // 현재 맵(plane)의 사이즈는 100x100

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;

        //float height = 2 * Camera.main.orthographicSize;
        //float width = height * Camera.main.aspect;

        //Vector2 pos = transform.position;
        //pos.x = Mathf.Clamp(pos.x, plane.xmin + width * 0.5f, plane.xmax - width * 0.5f);
        //pos.y = Mathf.Clamp(pos.x, plane.ymin + height * 0.5f, plane.ymax - height * 0.5f);

        //transform.position = pos;
    }

    private void CameraBoundary()
    {
        Vector3 dir = Vector3.zero;
        if (Input.mousePosition.y >= Screen.height - panBorderThickenss)
        {
            dir.z = 1;
        }
        if (Input.mousePosition.y <= panBorderThickenss)
        {
            dir.z = -1;
        }
        Vector3 upMove = forward * dir.z * Time.deltaTime;

        if (Input.mousePosition.x >= Screen.width - panBorderThickenss)
        {
            dir.x = 1;
        }
        if (Input.mousePosition.x <= panBorderThickenss)
        {
            dir.x = -1;
        }
        Vector3 rightMove = right * dir.x * Time.deltaTime;

        Vector3 heading = Vector3.Normalize(rightMove + upMove);

        if (dir.Equals(Vector3.zero)) CameraFollow();

        Vector3 pos = transform.position;
        pos += heading * panSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x + target.position.x , panLimit.x + target.position.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y + target.position.z, panLimit.y + target.position.z);

        transform.position = pos;
        //Vector3 pos = transform.position;
        //
        //if (Input.mousePosition.y >= Screen.height - panBorderThickenss)
        //    pos.z += panSpeed * Time.deltaTime;
        //if (Input.mousePosition.y <= panBorderThickenss)
        //    pos.z -= panSpeed * Time.deltaTime;
        //

        //if (Input.mousePosition.x >= Screen.width - panBorderThickenss)
        //    pos.x += panSpeed * Time.deltaTime;
        //if (Input.mousePosition.x <= panBorderThickenss)
        //    pos.x -= panSpeed * Time.deltaTime;
        //
        //pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        //pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        //
        //transform.position = pos;
    }

    private void Update()
    {
        if (target == null)
            return;

        CameraZoom();
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;
        //CameraFollow();
        CameraBoundary();
    }
}
