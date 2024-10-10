using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform puck;
    [SerializeField] private Transform OwnGoal;
    [SerializeField] private Transform OpponentGoal;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float rotateSpeed = 100f;

    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(10f, 5.3f, 0f);
        puck.localPosition = new Vector3(-7f, 3.5f, 0f);

        //rotate the agent 90 degrees on the y-axis
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        //give the puck a starting velocity
        puck.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);

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
            transform.Rotate(0f, moveRotate * rotateSpeed, 0f, Space.Self);
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

        if (other.TryGetComponent <Puck> (out Puck puck))
        {
            AddReward(3f);
        }

        if (other.gameObject.tag == "Puck")
        {
            AddReward(0.4f);
        }

        if (other.gameObject.tag == "Wall")
        {
            AddReward(-0.1f);
        }
    }

    public void ScoredAGoal()
    {
        // We use a reward of 5.
        AddReward(5f);

        // By marking an agent as done AgentReset() will be called automatically.
        EndEpisode();

    }
    


}