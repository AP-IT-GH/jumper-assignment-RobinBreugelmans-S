using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class JumperAgent : Agent
{
    [SerializeField] float jumpSpeed = 4;

    public override void OnEpisodeBegin()
    {
        resetPositionAndVelocity();
    }
    private void resetPositionAndVelocity()
    {
        transform.localPosition = new Vector3(0, .5f, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2
        int jumpPressed;
        jumpPressed = actionBuffers.DiscreteActions[0];
        if(jumpPressed >= 1)
        {
            GetComponent<Rigidbody>().linearVelocity = new Vector3(GetComponent<Rigidbody>().linearVelocity.x, jumpSpeed, GetComponent<Rigidbody>().linearVelocity.z);
        }

        /*
        if (transform.localPosition.y < 0)
        {
            AddReward(-2f);
            EndEpisode();
            return;
        }

        // target bereikt
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        if (distanceToTarget <= 1f)
        {
            AddReward(2f);
            Target.localPosition += Vector3.down * 4;
            return;
        }
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, goalLayer))
        {
            AddReward(1f);
            EndEpisode();
            return;
        }*/
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Convert.ToInt32(Input.GetButton("Jump"));
        Debug.Log(discreteActionsOut[0]);
    }
}
