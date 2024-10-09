using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(10f, 5.3f, 0f);
        target.localPosition = new Vector3(-7f, 3.5f, 0f);

        //rotate the agent 90 degrees on the y-axis
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
            float moveRotate = actions.ContinuousActions[0];
            float moveForward = actions.ContinuousActions[1];
            //float jump = actions.ContinuousActions[2];

            rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.fixedDeltaTime);
            transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
            //rb.AddForce(Vector3.up * jump * 1f, ForceMode.Impulse);



            // transform.localPosition += new UnityEngine.Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        if (continuousActions.Length >= 2)
        {
            continuousActions[0] = Input.GetAxis("Horizontal");
            continuousActions[1] = Input.GetAxis("Vertical");
            //continuousActions[2] = Input.GetAxis("Jump");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Puck")
        {
            AddReward(6f);
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}