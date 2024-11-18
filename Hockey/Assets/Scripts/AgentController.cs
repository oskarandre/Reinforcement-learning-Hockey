using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AgentMove : Agent
{
    [SerializeField] private Transform puck;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotateSpeed = 5f;

    public Rigidbody rb;

    public GoalDetectWithInput goalDetect;

    public bool stage1 = true;
    
    private float timepenalty;
    // Max_x = 27.86423 , Min_x = 8.021378 , Max_z = 7.825231, Min_z = -4.808197
    public float Max_x = 27.86423f;
    public float Max_z = 8.021378f;
    public float Min_x = 7.825231f;
    public float Min_z = -4.808197f;
    public float Max_puck_x = 5.65f; //5.307443
    public float Min_puck_x = -9.65f; //-9.409447
    public float Max_puck_z = 5.39586f;
    public float Min_puck_z = -4.076001f;

    public float goalpos_x = 5.94f;
    public float goalpos_z = 0.38f;

    public float puckImpactForce = 1f; // Adjustable force applied to the puck
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        goalDetect = puck.GetComponent<GoalDetectWithInput>();
        goalDetect.agent = this;

    }

    public override void OnEpisodeBegin()
    {

        timepenalty = 0f;

        if (stage1 == true) {
            transform.localPosition = new Vector3(Random.Range(16.75f,8.80f), 5.3f, Random.Range(-1.6f, 6.75f));
        
            //random position for the puck
            puck.localPosition = new Vector3(Random.Range(-1.8f, 5.0f), 3.5f, Random.Range(-2.8f, 4.45f));

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
        Quaternion rotation = transform.localRotation;
        Vector3 normalizedRotation = rotation.eulerAngles / 360.0f;  // [0,1]
        // Max_x = 27.86423 , Min_x = 8.021378 , Max_z = 7.825231, Min_z = -4.808197
        // normalizedValue = (currentValue - minValue)/(maxValue - minValue)
        //sensor.AddObservation(puck.transform.position - transform.position); 

        float normalized_x = Mathf.Clamp((transform.localPosition.x - Min_x) / (Max_x - Min_x), 0f, 1f);
        float normalized_z = Mathf.Clamp((transform.localPosition.z - Min_z) / (Max_z - Min_z), 0f, 1f);


        float normalize_puck_x = Mathf.Clamp((puck.localPosition.x - Min_puck_x) / (Max_puck_x - Min_puck_x), 0f, 1f);
        float normalize_puck_z = Mathf.Clamp((puck.localPosition.z - Min_puck_z) / (Max_puck_z - Min_puck_z), 0f, 1f);

        float normalize_goal_x = Mathf.Clamp((goalpos_x - Min_puck_x) / (Max_puck_x - Min_puck_x), 0f, 1f);
        float normalize_goal_z = Mathf.Clamp((goalpos_z - Min_puck_z) / (Max_puck_z - Min_puck_z), 0f, 1f);

        sensor.AddObservation(normalizedRotation); // 3?
        sensor.AddObservation(rb.velocity); // 3
        sensor.AddObservation(rb.angularVelocity); // 3 //GetAccumulatedTorque / force?
        sensor.AddObservation(normalized_x); // 1
        sensor.AddObservation(normalized_z); // 1
        sensor.AddObservation(normalize_puck_x - normalized_x); // 1
        //sensor.AddObservation(normalize_puck_z - normalized_z); // 1 added after MickJagger1 and 2
        sensor.AddObservation(puck.GetComponent<Rigidbody>().velocity); // 3
        sensor.AddObservation(normalize_goal_x - normalize_puck_x); // 1
        sensor.AddObservation(normalize_goal_z - normalize_puck_z); // 1        

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];
        
        
        // Apply force for movement
        Vector3 moveForce = transform.forward * moveForward * moveSpeed; 
        rb.AddForce(moveForce, ForceMode.Force);

        //rb.MovePosition(transform.position + transform.forward * moveForward * Time.deltaTime);


        // Apply torque for rotation
       // float torque = moveRotate * rotateSpeed;
       // rb.AddTorque(Vector3.up * torque, ForceMode.Force);

         // Calculate the target rotation based on moveRotate
        float rotationAngle = moveRotate * rotateSpeed * Time.deltaTime; // Rotate based on action input
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationAngle, 0f); // Rotation only around Y-axis

        // Apply the new rotation using MoveRotation
        rb.MoveRotation(rb.rotation * deltaRotation); 
        //rb.ResetInertiaTensor();
        
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
        timepenalty += 1/50000f;
      
      // fix for case of puck flying of map
        if(puck.GetComponent<Rigidbody>().position.y < 2f || rb.transform.localPosition.y < 4f) {
            SetReward(-1f);
            EndEpisode();
        }

    }

    // Apply additional torque upon collisions for dynamic response
     private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is the puck
    if (collision.gameObject.CompareTag("Puck"))
    {
        // Get the Rigidbody of the puck
        Rigidbody puckRb = collision.gameObject.GetComponent<Rigidbody>();

        if (puckRb != null)
        {
            // Calculate the rotational impact force
            Vector3 impactForce = (transform.forward * puckImpactForce);

            // Apply the force to the puck's Rigidbody at the collision point
            puckRb.AddForceAtPosition(impactForce, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
    } 


    private void OnTriggerEnter(Collider other)
    {   

        if (other.gameObject.tag == "Goal"){
            //AddReward(-0.1f);
            SetReward(-1f);
            EndEpisode();
        }

        if (other.gameObject.tag == "OwnGoal"){
            //AddReward(-0.1f);
            SetReward(-1f);
            EndEpisode();
        }

        if (other.gameObject.tag == "OpponentGoal"){
            //AddReward(-0.1f);
            SetReward(-1f);
            EndEpisode();
        }
        //Check if agent falls over
        if(other.gameObject.tag == "Ice") {
            SetReward(-1f);
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
                //AddReward(reward);
                //SetReward(reward - timepenalty);
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