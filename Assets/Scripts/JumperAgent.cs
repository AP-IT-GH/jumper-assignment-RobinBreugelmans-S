using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JumperAgent : Agent
{
    [SerializeField] float jumpSpeed = 6f;
    [SerializeField] LayerMask groundLayer;

    [Header("Objects")]
    [SerializeField] GameObject obstacle;
    [SerializeField] GameObject collectable;

    GameObject spawnedObject;
    bool spawnedObjectIsObstacle;
    bool hitObject;

    public override void OnEpisodeBegin()
    {
        hitObject = false;
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
        spawnedObject.transform.position = new Vector3(0, 0.5f, -5);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Move>() != null)
        {
            hitObject = true;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(spawnedObjectIsObstacle);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // 0 = don't jump, 1 = jump
        if(actionBuffers.DiscreteActions[0] == 1 &&
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
            if (!spawnedObjectIsObstacle)
            {
                //give reward when hitting collectable
                AddReward(1);
            }
            EndEpisode();
        }

        if (spawnedObject.transform.position.z >= 6)
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
