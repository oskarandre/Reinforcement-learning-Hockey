using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AgentMove : Agent
{
    [SerializeField] private Transform puck;
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 500f;

    public Rigidbody rb;

    public GoalDetectWithInput goalDetect;

    public bool stage1 = true;
    
    private float resetTimer;

    private float timepenalty;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        goalDetect = puck.GetComponent<GoalDetectWithInput>();
        goalDetect.agent = this;

    }

    public override void OnEpisodeBegin()
    {
        resetTimer = 0f;
        timepenalty = 0f;

        if (stage1 == true) {
            transform.localPosition = new Vector3(Random.Range(10f,19f), 5.3f, Random.Range(-1f, 1f));
        
            //random position for the puck
            puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-2f, 3f));

        }
        //rotate the agent 90 degrees on the y-axis
        rb.rotation = Quaternion.identity;

        //reset the rotation of puck
        puck.rotation = Quaternion.identity;

        //reset the velocity of the agent
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //reset the velocity of the puck
        puck.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(transform.localRotation);
        //sensor.AddObservation(puck.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
            float moveRotate = actions.ContinuousActions[0];
            float moveForward = actions.ContinuousActions[1];
            
            // Apply force for movement
            Vector3 moveForce = transform.forward * moveForward * moveSpeed; //TODO: Try rb.MovePosition?
            rb.AddForce(moveForce);

            // Apply torque for rotation
            float torque = moveRotate * rotateSpeed;
            rb.AddTorque(Vector3.up * torque);
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

    void FixedUpdate()
    {
        timepenalty += 1/500000f;
      
        if(puck.GetComponent<Rigidbody>().position.y < 2f) { // fix for case of puck flying of map
            SetReward(-1);
            EndEpisode();
        }
    }


    private void OnTriggerEnter(Collider other)
    {   

        if (other.gameObject.tag == "Goal"){
            //AddReward(-0.1f);
            SetReward(-1);
            EndEpisode();
        }

        if (other.gameObject.tag == "OwnGoal"){
            //AddReward(-0.1f);
            SetReward(-1);
            EndEpisode();
        }

        if (other.gameObject.tag == "OpponentGoal"){
            //AddReward(-0.1f);
            SetReward(-1);
            EndEpisode();
        }

      /*   if (other.gameObject.tag == "Puck")
        {
            Debug.Log("Puck detected");
            AddReward(0.1f);
        }

        if (other.gameObject.tag == "Wall")
        {
            AddReward(-0.05f);
            //EndEpisode();
        } */
    }

    public void ScoredAGoal(float reward) {

        if(reward > 0f){
            SetReward(reward - timepenalty);
        }
        else {
            SetReward(reward);
            }
        EndEpisode();
    }

    public void AgentReward(float reward, string type){

        if (stage1 == true) {
            if (type == "Stick"){
                AddReward(reward);
                //SetReward(reward);
                //EndEpisode();
            }
            if (type == "Time"){
                AddReward(reward);
            }

            if (type == "Wall"){
                AddReward(reward);
            }
        }
    }
}