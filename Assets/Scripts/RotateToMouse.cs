using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    // 카메라의 X축 회전 속도
    [SerializeField] private float rotCamXAxisSpeed = 5;
    
    // 카메라의 Y축 회전 속도
    [SerializeField] private float rotCamYAxisSpeed = 3;

    // 카메라 X축 회전 범위 (최소)
    private float limitMinX = -80;
    
    // 카메라 X축 회전 범위 (최대)
    private float limitMaxX = 50;
    
    private float eulerAngleX;
    private float eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        // 마우스의 좌 / 우 이동으로 카메라 Y축 회전
        eulerAngleY += mouseX * rotCamYAxisSpeed;
        
        // 마우스의 위 / 아래 이동으로 카메라 X축 회전
        eulerAngleX -= mouseY * rotCamXAxisSpeed;
        
        // 카메라의 X축 회전의 경우 회전 범위를 설정
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
