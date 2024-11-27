// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Actuators;
// using Unity.MLAgents.Sensors;

// public class AgentMove : Agent
// {
//     [SerializeField] private Transform puck;
//     [SerializeField] private float moveSpeed = 300f; // Reduced from 1000f for better control
//     [SerializeField] private float rotateSpeed = 150f; // Reduced from 500f for smoother rotation

//     public Rigidbody rb;
//     public Rigidbody puckRB;
//     public GoalDetectWithInput goalDetect;

    
//     // Select red team and variate stages
//     public bool redTeam = false;

//     // Select blue team and no Stages
//     public bool blueTeam = false;

//     private float resetTimer = 0f;

//     private bool redGoal = false;
//     private bool blueGoal = false;

//     public override void Initialize()
//     {
//         rb = GetComponent<Rigidbody>();
//         puckRB = puck.GetComponent<Rigidbody>(); 
//         goalDetect = puck.GetComponent<GoalDetectWithInput>();
//         goalDetect.agent = this;
//     }


//     public override void OnEpisodeBegin()
//     {

//         transform.rotation = Quaternion.Euler(0f, 90f, 0f);
//         puck.rotation = Quaternion.Euler(0f, 0f, 0f);
//         rb.velocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         puckRB.velocity = Vector3.zero;
//         resetTimer = 0f;

        
//         // Randomize agent and puck position with even more variety for more complex learning
//         transform.localPosition = new Vector3(12f, 5.3f, 1.4f);
//         puck.localPosition = new Vector3(2.5f, 3.5f, 1f);

//         // Randomize rotation for more variety
//         transform.rotation = Quaternion.Euler(0f,0f, 0f);        
        
//     }

//     public override void OnActionReceived(ActionBuffers actions)
//     {
//         float moveRotate = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
//         float moveForward = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        
//         if (!redGoal && !blueGoal){
//             Vector3 moveForce = transform.forward * moveForward * moveSpeed;
//             rb.AddForce(moveForce);

//             float torque = moveRotate * rotateSpeed;
//             rb.AddTorque(Vector3.up * torque);
//         }
//     }

//     public override void Heuristic(in ActionBuffers actionsOut)
//     {
//         ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
//         if (continuousActions.Length >= 2)
//         {
//             continuousActions[0] = Input.GetAxis("Horizontal");
//             continuousActions[1] = Input.GetAxis("Vertical");
//         }
//     }

//     void Update()
//     {
//         // Time penalty to encourage efficiency
        
//         resetTimer += Time.deltaTime;
//     }

        

//     public void ScoredAGoal(float reward)
//     {
//         if(redTeam)
//         {
//             redGoal = true;
//             print("Red Goal Time: " + resetTimer);
//         }
//         if(blueTeam)
//         {
//             blueGoal = true;
//             print("Blue Goal Time: " + resetTimer);
//         }
//     }

//     public void AgentReward(float reward, string type)
//     {
//     }
// }




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform puck;
    [SerializeField] private float moveSpeed = 300f; // Reduced from 1000f for better control
    [SerializeField] private float rotateSpeed = 150f; // Reduced from 500f for smoother rotation

    public Rigidbody rb;
    public Rigidbody puckRB;
    public GoalDetectWithInput goalDetect;

    // Different stages for different levels of complexity
    // Do 5m steps for each stage

    // Stage 1: Agent and puck close to the goal for faster learning
    // Reward for moving forward and penalty for sticking to walls
    public bool stage1 = true;
    
    // Stage 2: Agent and puck position with less chance of being close to the goal
    // Reward for moving forward and penalty for sticking to walls
    public bool stage2 = false;

    // Stage 3: Agent and puck position with more variety for more complex learning
    // No reward for moving forward or sticking to walls, only time penalty and goal touching penalty
    public bool stage3 = false;

    // Stage 4: Agent and puck position with even more variety for more complex learning
    public bool stage4 = false;

    public bool noStages = false;

    // Select red team and variate stages
    public bool redTeam = false;

    // Select blue team and no Stages
    public bool blueTeam = false;

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

        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        puck.rotation = Quaternion.Euler(0f, 0f, 0f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        puckRB.velocity = Vector3.zero;
        resetTimer = 0f;

        if (noStages)
        {
            // Randomize agent and puck position with even more variety for more complex learning
            transform.localPosition = new Vector3(Random.Range(10f, 26f), 5.3f, Random.Range(-3f, 6f));
            puck.localPosition = new Vector3(Random.Range(-8f, 3f), 3.5f, Random.Range(-4f, 4.2f));

            // Randomize rotation for more variety
            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            puck.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        else if (stage1)
        {
            // Randomize agent and puck close to the goal for faster learning
            if (Random.Range(0, 3) == 0)
            {
                transform.localPosition = new Vector3(Random.Range(23f, 25f), 5.3f, Random.Range(0f, 4f));
                puck.localPosition = new Vector3(Random.Range(4f, 5f), 3.5f, Random.Range(-1.5f, 2.5f));
            }
            else
            {
                transform.localPosition = new Vector3(Random.Range(10f, 23f), 5.3f, Random.Range(-2f, 5f));
                puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-0.5f, 1.5f));
            }
            
        }
        else if (stage2)
        {
            // Randomize agent and puck position with less chance of being close to the goal
            if (Random.Range(0, 5) == 0)
            {
                transform.localPosition = new Vector3(Random.Range(23f, 25f), 5.3f, Random.Range(0f, 4f));
                puck.localPosition = new Vector3(Random.Range(4f, 5f), 3.5f, Random.Range(-1.5f, 2.5f));
            }
            else
            {
                transform.localPosition = new Vector3(Random.Range(10f, 23f), 5.3f, Random.Range(-2f, 5f));
                puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-0.5f, 1.5f));
            }
        }

        else if(stage3)
        {
            // Randomize agent and puck position with more variety for more complex learning
            transform.localPosition = new Vector3(Random.Range(10f, 26f), 5.3f, Random.Range(-3f, 6f));
            puck.localPosition = new Vector3(Random.Range(-8f, 3f), 3.5f, Random.Range(-2f, 2.2f));
        }

        else if (stage4)
        {
            // Randomize agent and puck position with even more variety for more complex learning
            transform.localPosition = new Vector3(Random.Range(10f, 26f), 5.3f, Random.Range(-3f, 6f));
            puck.localPosition = new Vector3(Random.Range(-8f, 3f), 3.5f, Random.Range(-4f, 4.2f));

            // Randomize rotation for more variety
            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            puck.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveForward = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 moveForce = transform.forward * moveForward * moveSpeed;
        rb.AddForce(moveForce);

        float torque = moveRotate * rotateSpeed;
        rb.AddTorque(Vector3.up * torque);

        if (stage1 || stage2 || noStages)
        {
            // Encourage smoother movement and reward progress
            if (moveForward > 0.1f)
            {
                AddReward(0.001f); // Small reward for moving forward
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        if (continuousActions.Length >= 2)
        {
            continuousActions[0] = Input.GetAxis("Horizontal");
            continuousActions[1] = Input.GetAxis("Vertical");
        }
    }

    void Update()
    {
        // Time penalty to encourage efficiency
        
        resetTimer += Time.deltaTime;
        if(noStages)
        {
            AddReward(-0.004f);
            if (resetTimer > 120f) // Adjusted threshold for episode length
            {
                AddReward(-1f); // Strong penalty for taking too long
                EndEpisode();
            }
        }
        else if(stage1)
        {
            AddReward(-0.004f);
            if (resetTimer > 60f) // Adjusted threshold for episode length
            {
                AddReward(-1f); // Strong penalty for taking too long
                EndEpisode();
            }
        }
        else if(stage4)
        {
            AddReward(-0.004f);
            if (resetTimer > 120f) // Adjusted threshold for episode length
            {
                AddReward(-1f); // Strong penalty for taking too long
                EndEpisode();
            }
        }
        else{
            AddReward(-0.004f);
            if (resetTimer > 80f) // Adjusted threshold for episode length
            {
                AddReward(-1f); // Strong penalty for taking too long
                EndEpisode();
            }
    }

        
    }

    private void OnTriggerStay(Collider other)
    {
        if ((stage1 || stage2 || noStages) && other.CompareTag("Wall"))
        {
            AddReward(-0.001f); // Negative reward for sticking to walls

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("redGoal"))
        {
            AddReward(-0.1f); // Strong penalty for own goal
            if (stage1 || stage2)
            {
                EndEpisode();
            }
        }

        if (other.CompareTag("blueGoal"))
        {
            AddReward(-0.1f); // Reward for scoring in the opponent's goal
            if (stage1 || stage2)
            {
                EndEpisode();
            }
        }
    }

    public void ScoredAGoal(float reward)
    {
        if(redTeam)
        {
            AddReward(reward);
        }
        if(blueTeam)
        {
            AddReward(-reward);
        }
        EndEpisode();
    }

    public void AgentReward(float reward, string type)
    {
        if (stage1 && type == "Stick")
        {
            AddReward(reward);
        }
        else if (noStages && type == "Stick")
        {
            AddReward(0.1f);
        }
        else if (stage2 && type == "Stick")
        {
            AddReward(reward * 0.1f);
        }
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Actuators;
// using Unity.MLAgents.Sensors;


// public class AgentMove : Agent
// {
//     [SerializeField] private Transform puck;
//     [SerializeField] private float moveSpeed = 1000f;
//     [SerializeField] private float rotateSpeed = 500f;

//     public Rigidbody rb;

//     public Rigidbody puckRB;

//     public GoalDetectWithInput goalDetect;

//     public bool stage1 = true;
    
//     public bool stage2 = false;

//     public bool stage3 = false;

//     private float resetTimer = 0f;


//     public override void Initialize()
//     {
//         rb = GetComponent<Rigidbody>();
//         puckRB = puck.GetComponent<Rigidbody>(); 
//         goalDetect = puck.GetComponent<GoalDetectWithInput>();
//         goalDetect.agent = this;
//     }

//     public override void OnEpisodeBegin()
//     {
//         if (stage1 == true) {

//             var playerPosition = Random.Range(0, 4);

//             if (playerPosition == 0){
//                 transform.localPosition = new Vector3(Random.Range(23f,25f), 5.3f, Random.Range(0f, 4f));
//                 //random position for the puck
//                 puck.localPosition = new Vector3(Random.Range(4f, 5f), 3.5f, Random.Range(-1.5f, 2.5f));
//             }
//             else {
//                 transform.localPosition = new Vector3(Random.Range(10f,23f), 5.3f, Random.Range(-2f, 5f));

//                 //random position for the puck
//                 puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-0.5f, 1.5f));
//             }

//             resetTimer = 0f;
            

//             // transform.localPosition = new Vector3(Random.Range(15f,17f), 5.3f, Random.Range(-1f, 1f));
        
//             // //random position for the puck
//             // puck.localPosition = new Vector3(Random.Range(-2f, 2f), 3.5f, Random.Range(-2f, 3f));

//         }

//         if (stage2 == true) {

//             transform.localPosition = new Vector3(Random.Range(10f,19f), 5.3f, Random.Range(-1f, 1f));
        
//             //random position for the puck
//             puck.localPosition = new Vector3(Random.Range(-7f, 4f), 3.5f, Random.Range(-2f, 3f));
//         }

//         //rotate the agent 90 degrees on the y-axis
//         transform.rotation = Quaternion.Euler(0f, 90f, 0f);

//         //reset the rotation of puck
//         puck.rotation = Quaternion.Euler(0f, 0f, 0f);

//         //reset the velocity of the agent
//         rb.velocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;

//         //reset the velocity of the puck
//         puck.GetComponent<Rigidbody>().velocity = Vector3.zero;


//         //give the puck a starting velocity (debugging)
//         //puck.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);

//     }

//     public override void OnActionReceived(ActionBuffers actions)
//     {
//             float moveRotate = actions.ContinuousActions[0];
//             float moveForward = actions.ContinuousActions[1];
            
//             // Apply force for movement
//             Vector3 moveForce = transform.forward * moveForward * moveSpeed;
//             rb.AddForce(moveForce);

//             // Apply torque for rotation
//             float torque = moveRotate * rotateSpeed;
//             rb.AddTorque(Vector3.up * torque);
//             }

//     public override void Heuristic(in ActionBuffers actionsOut)
//     {
//         ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
//         if (continuousActions.Length >= 2)
//         {
//             continuousActions[0] = Input.GetAxis("Horizontal");
//             continuousActions[1] = Input.GetAxis("Vertical");
//             //continuousActions[2] = Input.GetAxis("Jump");
//         }
//     }

//     void Update()
//     {
//         AgentReward(-0.0002f, "Time");

//         resetTimer += 0.0002f;


//         // if (puckRB.velocity.magnitude < 0.01f && rb.velocity.magnitude < 0.01f){
//         //     //AddReward(-0.0025f);
//         //     resetTimer += 0.002f;
//         // }

//         if (resetTimer > 1f){
//             SetReward(-1f);
//             EndEpisode();
//         }

//     }

//     private void OnTriggerStay(Collider other)
//     {
//         if (other.gameObject.tag == "Wall"){
//             //AddReward(-0.001f);
//             //EndEpisode();
//             //Debug.Log("Wall detected");
//         }

//         if (other.gameObject.tag == "OwnGoal"){
//             //AddReward(-0.001f);
//             //EndEpisode();
//         }

//         if (other.gameObject.tag == "OpponentGoal"){
//             //AddReward(-0.001f);
//             //EndEpisode();
//         }

//     }

//     private void OnTriggerEnter(Collider other)
//     {   

//         // if (other.TryGetComponent <Puck> (out Puck puck))
//         // {
//         //     AddReward(3f);
//         // }

//         if (other.gameObject.tag == "Goal"){
//             //AddReward(-0.05f);
//             //EndEpisode();
//         }

        

//         // if (other.gameObject.tag == "Puck")
//         // {
//         //     //Debug.Log("Puck detected");
//         //     AddReward(1f);
//         //     EndEpisode();
//         // }

//         // if (other.gameObject.tag == "Wall")
//         // {
//         //     AddReward(-0.05f);
//         //     //EndEpisode();
//         // }
//     }


//     public void ScoredAGoal(float reward)
//     {
//         AddReward(reward);
//         //Debug.Log("Goal Scored! " + reward);
//         // By marking an agent as done AgentReset() will be called automatically.
//         EndEpisode();
//     }

//     public void AgentReward(float reward, string type){

//         if (stage1 == true) {
//             if (type == "Stick"){
//                 AddReward(reward);
//                 //EndEpisode();
//             }
//             if (type == "Time"){
//                 AddReward(reward);
//             }
//         }

//         if (stage2 == true) {
//             if (type == "Stick"){
//                 AddReward(reward*0.25f);
//             }
//             if (type == "Puck"){
//                 AddReward(reward);
//             }
//             if (type == "Time"){
//                 AddReward(reward*2f);
//             }
//         }
//     }

    
// }