using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAgent : Agent
{
    public Transform playerTransform;
    public GameObject obstaclePrefab; // Prefab for obstacles
    public float moveSpeed = 10f; // Obstacle movement speed
    public float moveSpeedPlayer = 10f; // Player movement speed
    public Transform finishLine;
    public Transform parentTransform; // Parent transform for obstacles
    public int maxObstacles = 8; // Maximum number of obstacles on each side
    public List<GameObject> checkpointsList = new List<GameObject>();
    private int checkpointCoutner = 0;
    public Material winMaterial;
    public Material LoseMaterial;
    public Renderer materialHolderRenderer;
    public RayPerceptionSensorComponent2D rayPerceptionSensorComponent;

    private void Start(){
        // rayPerceptionSensorComponent = GetComponent<RayPerceptionSensor>();
    
    }
    private readonly Vector3[] directions = new Vector3[]
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.down
    };
    // private void OnDrawGizmos(){
    //     var sensor = GetComponent<RayPerceptionSensorComponent2D>();

    //     var rayinput = sensor.GetRayPerceptionInput();
    //     // if(rayinput) return;

    //     // foreach(var rayInfo in rayinput.rayinfo)
    // }
  

    private List<GameObject> obstacles = new List<GameObject>();

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions.Clear();

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 3;
        }
        else
        {
            discreteActions[0] = -1;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;
        int action = discreteActions[0];

        if (action >= 0 && action < directions.Length)
        {
            MovePlayer(directions[action]);
        }
        AddReward(-0.008f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(playerTransform.localPosition);
        sensor.AddObservation(finishLine.localPosition);
        
        
        float distanceToFinish = Vector3.Distance(playerTransform.localPosition, finishLine.localPosition);
        float proximityReward = Mathf.Clamp01(1 - distanceToFinish / 10f); // Normalize distance
        AddReward(proximityReward * 0.01f);
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        InitializeObstacles();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void MovePlayer(Vector3 direction)
    {
        playerTransform.localPosition += direction * moveSpeedPlayer * Time.deltaTime;
    }

    private void HandleCollision(Collider2D collision)
    {

        if (collision.CompareTag("Obstacle") || collision.CompareTag("Walls"))
        {
            Debug.Log("Obstacle");
            AddReward(-1f);
            //materialHolderRenderer.material = LoseMaterial;
            EndEpisode();
        }
        else if (collision.CompareTag("Finish"))
        {
            Debug.Log("Finish Line");
            AddReward(10f);
            //materialHolderRenderer.material = winMaterial;
            EndEpisode();
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint");
            
            AddReward(1);
            collision.gameObject.SetActive(false);
        }
    }

    private void ResetPlayer()
    {
        playerTransform.localPosition = new Vector3(Random.Range(-7, 7), -9, 0);
    }

    private void InitializeObstacles()
    {
        // Destroy existing obstacles
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        obstacles.Clear();

        // Define the y positions in a repeating pattern
        float[] yPositions = { -6f, -2f, 3f, 7f };

        // Generate new obstacles
        for (int i = 0; i < maxObstacles; i++)
        {
            // Calculate y position based on index
            float y = yPositions[i % yPositions.Length];

            // Create positions for left and right obstacles
            Vector3 leftSpawnPosition = new Vector3(Random.Range(-8f, 8f), y, 0);
            Vector3 rightSpawnPosition = new Vector3(Random.Range(-8f, 8f), y, 0);

            // Spawn obstacles
            SpawnObstacle(leftSpawnPosition, Vector3.right);
            SpawnObstacle(rightSpawnPosition, Vector3.left);
        }
    }

    private void SpawnObstacle(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject obstacle = Instantiate(obstaclePrefab);
        obstacle.transform.SetParent(parentTransform, false);
        obstacle.transform.localPosition = spawnPosition; // Use position instead of localPosition
        obstacles.Add(obstacle);
        StartCoroutine(MoveObstacle(obstacle.transform, direction));
    }



    private IEnumerator MoveObstacle(Transform obstacle, Vector3 direction)
    {
        while (true)
        {
            if (obstacle == null)
            {
                yield break;
            }

            obstacle.localPosition += direction * Time.deltaTime * moveSpeed;

            // Check if the obstacle has moved out of bounds based on its local position relative to the parent
            if (Mathf.Abs(obstacle.localPosition.x) > 9)
            {
                if (direction == Vector3.right)
                {
                    obstacle.localPosition = new Vector3(-8f, obstacle.localPosition.y, 0);
                }
                else
                {
                    obstacle.localPosition = new Vector3(8f, obstacle.localPosition.y, 0);
                }
            }

            yield return null;
        }
    }
}
