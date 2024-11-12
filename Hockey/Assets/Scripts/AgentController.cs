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

    public Rigidbody puckRB;

    public GoalDetectWithInput goalDetect;

    public bool stage1 = true;
    
    public bool stage2 = false;

    public bool stage3 = false;

    private float resetTimer = 0f;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        puckRB = puck.GetComponent<Rigidbody>(); 
        goalDetect = puck.GetComponent<GoalDetectWithInput>();
        goalDetect.agent = this;
    }

    public override void OnEpisodeBegin()
    {
        if (stage1 == true) {

            var playerPosition = Random.Range(0, 2);

            if (playerPosition == 0){
                transform.localPosition = new Vector3(Random.Range(23f,25f), 5.3f, Random.Range(0f, 4f));
                //random position for the puck
                puck.localPosition = new Vector3(Random.Range(4f, 5f), 3.5f, Random.Range(-1.5f, 2.5f));
            }
            else {
                transform.localPosition = new Vector3(Random.Range(10f,23f), 5.3f, Random.Range(-2f, 5f));

                //random position for the puck
                puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-0.5f, 1.5f));
            }

            resetTimer = 0f;
            

            // transform.localPosition = new Vector3(Random.Range(15f,17f), 5.3f, Random.Range(-1f, 1f));
        
            // //random position for the puck
            // puck.localPosition = new Vector3(Random.Range(-2f, 2f), 3.5f, Random.Range(-2f, 3f));

        }

        if (stage2 == true) {

            transform.localPosition = new Vector3(Random.Range(10f,19f), 5.3f, Random.Range(-1f, 1f));
        
            //random position for the puck
            puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-2f, 3f));
        }

        //rotate the agent 90 degrees on the y-axis
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        //reset the rotation of puck
        puck.rotation = Quaternion.Euler(0f, 0f, 0f);

        //reset the velocity of the agent
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //reset the velocity of the puck
        puck.GetComponent<Rigidbody>().velocity = Vector3.zero;


        //give the puck a starting velocity (debugging)
        //puck.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
            float moveRotate = actions.ContinuousActions[0];
            float moveForward = actions.ContinuousActions[1];
            
            // Apply force for movement
            Vector3 moveForce = transform.forward * moveForward * moveSpeed;
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

    void Update()
    {
        AgentReward(-0.0007f, "Time");

        resetTimer += 0.0007f;


        // if (puckRB.velocity.magnitude < 0.01f && rb.velocity.magnitude < 0.01f){
        //     //AddReward(-0.0025f);
        //     resetTimer += 0.002f;
        // }

        if (resetTimer > 10f){
            SetReward(-1f);
            EndEpisode();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Wall"){
            //AddReward(-0.001f);
            //EndEpisode();
            //Debug.Log("Wall detected");
        }

        if (other.gameObject.tag == "OwnGoal"){
            //AddReward(-0.001f);
            //EndEpisode();
        }

        if (other.gameObject.tag == "OpponentGoal"){
            //AddReward(-0.001f);
            //EndEpisode();
        }

    }

    private void OnTriggerEnter(Collider other)
    {   

        // if (other.TryGetComponent <Puck> (out Puck puck))
        // {
        //     AddReward(3f);
        // }

        if (other.gameObject.tag == "Goal"){
            //AddReward(-0.05f);
            //EndEpisode();
        }

        

        // if (other.gameObject.tag == "Puck")
        // {
        //     //Debug.Log("Puck detected");
        //     AddReward(1f);
        //     EndEpisode();
        // }

        // if (other.gameObject.tag == "Wall")
        // {
        //     AddReward(-0.05f);
        //     //EndEpisode();
        // }
    }


    public void ScoredAGoal(float reward)
    {
        AddReward(reward);
        //Debug.Log("Goal Scored! " + reward);
        // By marking an agent as done AgentReset() will be called automatically.
        EndEpisode();
    }

    public void AgentReward(float reward, string type){

        if (stage1 == true) {
            if (type == "Stick"){
                AddReward(reward);
                //EndEpisode();
            }
            if (type == "Time"){
                AddReward(reward);
            }
        }

        if (stage2 == true) {
            if (type == "Stick"){
                AddReward(reward*0.25f);
            }
            if (type == "Puck"){
                AddReward(reward);
            }
            if (type == "Time"){
                AddReward(reward*2f);
            }
        }
    }

    
}