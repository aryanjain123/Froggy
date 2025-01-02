using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleNew : MonoBehaviour
{
    public Transform[] leftObstacles;
    public Transform[] rightObstacles;
    public float moveSpeed = 10f;

    void Start()
    {
        int y = -5;
        for (int i = 0; i < leftObstacles.Length; i++)
        {
            Transform leftObstacle = leftObstacles[i];
            Transform rightObstacle = rightObstacles[i];
            leftObstacle.localPosition = new Vector2(-8, y);
            rightObstacle.localPosition = new Vector2(8, y);
            y += 4;
            if (y > 7)
            {
                y = -5;
            }
        }

        for (int i = 0; i < leftObstacles.Length; i++)
        {
            float startDelayLeft = Random.Range(0.1f, 1.7f);
            float startDelayRight = Random.Range(0.3f, 2f);
            StartCoroutine(StartObstacleMovement(leftObstacles[i], startDelayLeft, Vector3.right, -8));
            StartCoroutine(StartObstacleMovement(rightObstacles[i], startDelayRight, Vector3.left, 8));
        }
    }

    IEnumerator StartObstacleMovement(Transform obstacle, float delay, Vector3 direction, float resetPositionX)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            obstacle.localPosition += direction * Time.deltaTime * moveSpeed;

            if (Mathf.Abs(obstacle.localPosition.x) > 9)
            {
                obstacle.localPosition = new Vector2(resetPositionX, obstacle.localPosition.y);
            }

            yield return null;
        }
    }
}
