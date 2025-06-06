using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        // ������� ����� ��������
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        // ³��������� ��������������� ����
        Vector3 forward = fov.transform.forward;
        Vector3 right = fov.transform.right;

        Vector3 dirLeft = Quaternion.Euler(0, -fov.horizontalAngle / 2, 0) * forward;
        Vector3 dirRight = Quaternion.Euler(0, fov.horizontalAngle / 2, 0) * forward;

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + dirLeft * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + dirRight * fov.radius);

        // ³��������� ������������� ���� �� ��� ���
        Vector3 dirLeftUp = Quaternion.Euler(-fov.verticalAngle / 2, -fov.horizontalAngle / 2, 0) * forward;
        Vector3 dirLeftDown = Quaternion.Euler(fov.verticalAngle / 2, -fov.horizontalAngle / 2, 0) * forward;

        Handles.color = new Color(1f, 0.5f, 0f); // ���������
        Handles.DrawLine(fov.transform.position, fov.transform.position + dirLeftUp * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + dirLeftDown * fov.radius);

        // ³��������� ������������� ���� �� ����� ���
        Vector3 dirRightUp = Quaternion.Euler(-fov.verticalAngle / 2, fov.horizontalAngle / 2, 0) * forward;
        Vector3 dirRightDown = Quaternion.Euler(fov.verticalAngle / 2, fov.horizontalAngle / 2, 0) * forward;

        Handles.DrawLine(fov.transform.position, fov.transform.position + dirRightUp * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + dirRightDown * fov.radius);

        // ���� ������ ������ � ��� �� �����
        if (fov.canSeePlayer && fov.playerRef != null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }
}
