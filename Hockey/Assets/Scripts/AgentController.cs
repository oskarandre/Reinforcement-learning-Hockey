using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AgentMove : Agent
{
    [SerializeField] private Transform puck;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float rotateSpeed = 100f;

    private Rigidbody rb;

    public GoalDetectWithInput goalDetect;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        goalDetect = puck.GetComponent<GoalDetectWithInput>();
        goalDetect.agent = this;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(10f, 5.3f, 0f);
        
        //random position for the puck
        puck.localPosition = new Vector3(Random.Range(-5f, 1f), 3.5f, Random.Range(-2f, 4f));

        //rotate the agent 90 degrees on the y-axis
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        //reset the rotation of puck
        puck.rotation = Quaternion.Euler(0f, 0f, 0f);

        //reset the velocity of the agent
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //reset the velocity of the puck
        puck.GetComponent<Rigidbody>().velocity = Vector3.zero;


        //give the puck a starting velocity
        //puck.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);

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

        // if (other.TryGetComponent <Puck> (out Puck puck))
        // {
        //     AddReward(3f);
        // }

        if (other.gameObject.tag == "Goal"){
            AddReward(-0.1f);
            EndEpisode();
        }

        if (other.gameObject.tag == "Puck")
        {
            //Debug.Log("Puck detected");
            AddReward(0.4f);
            //EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            //Debug.Log("Wall detected");
            AddReward(-0.1f);
            //EndEpisode();
        }
    }

    public void ScoredAGoal(float reward)
    {
        AddReward(reward);
        // By marking an agent as done AgentReset() will be called automatically.
        EndEpisode();
    }

    
}