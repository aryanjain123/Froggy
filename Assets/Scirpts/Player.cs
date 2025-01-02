using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Player : MonoBehaviour
{
    //grid movement = 1unit
    public float moveSpeed = 2.0f;
    public Transform transformPlayer;

    void Start()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transformPlayer.position += Vector3.up * moveSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transformPlayer.position += Vector3.right * moveSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            transformPlayer.position += Vector3.left * moveSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            transformPlayer.position += Vector3.down * moveSpeed;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            //give Penalty
            ResetPosition();
        }
        if (collision.tag == "Finish")
        {
            //give Reward
            ResetPosition();
        }
    }
    public void ResetPosition()
    {
        transformPlayer.position = new Vector3(Random.Range(-7, 7), -9, 0);
    }
}
