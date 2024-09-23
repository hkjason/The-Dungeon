using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDrawer : MonoBehaviour
{
    private float _horizontalSpeed;
    private float _verticalSpeed;
    private Vector3 direction;
    private void Update()
    {
        _horizontalSpeed = Input.GetAxis("Horizontal");
        _verticalSpeed = Input.GetAxis("Vertical");
        direction = new Vector3(_horizontalSpeed, 0, _verticalSpeed);
    }
    void OnDrawGizmosSelected()
    {
  
        //Vector3 crossProduct = Vector3.Cross(direction.normalized, Vector3.left).magnitude * direction.normalized * 5;
        Vector3 drawRay = direction * 5;

        Vector3 maxSqure = direction.normalized * ExtensionMethods.Remap(direction.magnitude, 0, Mathf.Sqrt(_horizontalSpeed * _horizontalSpeed + _verticalSpeed * _verticalSpeed), 0, 1)* 5;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, drawRay);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.left*10, maxSqure);
    }
}
