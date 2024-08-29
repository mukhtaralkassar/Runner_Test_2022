using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private PlayerController _CameraTarget;
    private Vector3 _DesiredPosition, _CurrentVelocity, _SmoothedPosition;
    public Vector3 Offset = new Vector3(0, 4, -5);
    public float SmoothSpeed = 0.125f;
    private void Start()
    {
        _CameraTarget = PlayerController.Instance;
    }
    private void LateUpdate()
    {
        if (_CameraTarget != null)
        {

            if (_CameraTarget.CurrentLane == 0)
            {
                _DesiredPosition = new Vector3(0, 0, _CameraTarget.transform.position.z) + Offset;
            }
            else if (_CameraTarget.CurrentLane == -1)
            {
                _DesiredPosition = new Vector3(-1, 0, _CameraTarget.transform.position.z) + Offset;
            }
            else if (_CameraTarget.CurrentLane == 1)
            {
                _DesiredPosition = new Vector3(1, 0, _CameraTarget.transform.position.z) + Offset;
            }
            _SmoothedPosition = Vector3.Lerp(transform.position, _DesiredPosition, SmoothSpeed * Time.deltaTime);
            _SmoothedPosition.z = _DesiredPosition.z;
            transform.position = _SmoothedPosition;
        }
    }
}
