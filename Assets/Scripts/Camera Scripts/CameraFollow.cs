using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float follow_Height = 8f;
    public float follow_Distance = 6f;

    public Transform player;

    private float target_Height;
    private float current_Height;
    private float current_Rotation;

    void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        target_Height = player.position.y + follow_Height;
        current_Rotation = transform.eulerAngles.y;
        current_Height = Mathf.Lerp(transform.position.y, target_Height, 0.9f * Time.deltaTime);
        Quaternion eulerY = Quaternion.Euler(0f, current_Rotation, 0f);
        Vector3 target_Position = player.position - (eulerY * Vector3.forward) * follow_Distance;
        target_Position.y = current_Height;
        transform.position = target_Position;
        transform.LookAt(player);
    }
}
