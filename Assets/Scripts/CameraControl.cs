using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Transform lookAt;
    public Transform camTransform;
    [SerializeField] float m_distance = 10.0f;

    [SerializeField] float m_currentX = 0.0f;
    [SerializeField] float m_currentY = 45.0f;
    [SerializeField] float m_sensitivityX = 4.0f;
    [SerializeField] float m_sensitivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
    }

    private void Update()
    {
        m_currentX += Input.GetAxis("Mouse X");
        m_currentY += Input.GetAxis("Mouse Y");

        m_currentY = Mathf.Clamp(m_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -m_distance);
        Quaternion rot = Quaternion.Euler(m_currentY, m_currentX, 0);
        camTransform.position = lookAt.position + rot * dir;
        camTransform.LookAt(lookAt.position);
    }
}
