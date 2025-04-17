using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JumperAgent : Agent
{

    [SerializeField] float jumpSpeed;
    GameObject spawnedObject;
    [SerializeField] LayerMask groundLayer;
    bool spawnedObjectIsObstacle;

    [Header("Objects")]
    [SerializeField] GameObject obstacle;
    [SerializeField] GameObject collectable;

    public bool hitObject;

    public override void OnEpisodeBegin()
    {
        Destroy(spawnedObject);
        resetPositionAndVelocity();
        spawnObstacleOrCollectable();
    }
    private void resetPositionAndVelocity()
    {
        transform.localPosition = new Vector3(0, .5f, 4);
        transform.localRotation = Quaternion.Euler(0, 180, 0);
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
    
    private void spawnObstacleOrCollectable()
    {
        if (Random.value >= .5f)
        {
            spawnedObject = Instantiate(obstacle);
            spawnedObjectIsObstacle = true;
        }
        else
        {
            spawnedObject = Instantiate(collectable);
            spawnedObjectIsObstacle = false;
        }
        if (Random.value >= .5f)
            spawnedObject.transform.position += Vector3.up * 1.2f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(spawnedObjectIsObstacle);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int jumpPressed;
        jumpPressed = actionBuffers.DiscreteActions[0];
        if(jumpPressed >= 1 &&
            Physics.Raycast(
                transform.position,
                Vector3.down,
                .6f,
                groundLayer
            )
        )
        {
            GetComponent<Rigidbody>().linearVelocity = new Vector3(GetComponent<Rigidbody>().linearVelocity.x, jumpSpeed, GetComponent<Rigidbody>().linearVelocity.z);
        }
        
        if (hitObject)
        {
            hitObject = false;
            if (spawnedObjectIsObstacle)
            {
                AddReward(-1);
                EndEpisode();
            }
            else
            {
                AddReward(1);
                EndEpisode(); //needed?
            }
        }

        if (spawnedObject.transform.position.z >= 5)
        {
            Destroy(spawnedObject);
            endEpisodeAndGiveReward();
        }

        //failsafe
        if (spawnedObject == null )
        {
            endEpisodeAndGiveReward();
        }
    }

    private void endEpisodeAndGiveReward()
    {
        if (spawnedObjectIsObstacle)
        {
            //didn't touch obstacle during episode
            AddReward(1);
        } else
        {
            //didn't collect collectable
            AddReward(-1);
        }
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = System.Convert.ToInt32(Input.GetKey(KeyCode.Space));
        
        if(spawnedObject == null)
        {
            spawnObstacleOrCollectable();
        }
    }
}
