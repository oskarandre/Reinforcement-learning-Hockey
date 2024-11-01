using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{

    [SerializeField] private Transform OwnGoal;
    [SerializeField] private Transform OpponentGoal;

    public AgentMove agent; 
    public float maxstep = 500f; // Define maxstep
    private float tick = 0f;
    private float angle_tolerance = 15f; // tolerance for diff between velocity and direction 

    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Stick_pos = agent.transform.GetChild(6).transform.position;
        Vector3 GoalPos = OpponentGoal.position;
        Vector3 PuckPos = transform.position;

        CalcReward_RB_to_POSv2(GoalPos, GetComponent<Rigidbody>()); 
        CalcReward_RB_to_POS(PuckPos, agent.GetComponent<Rigidbody>());
        
        //CalcReward_Puck_to_Goal(PuckPos, agent.GetComponent<Rigidbody>().transform.position);

        //the closer the puck is to the goal, the higher the reward
    
        // Reward based on Velocity of puck in relation to direction of opponentGoal
        
        //normalize the distance
        //distanceToOpponentGoal = distanceToOpponentGoal / 15f;

        float puck_oppgoal_distance = Vector3.Distance(GoalPos,  PuckPos);

        //CalcReward(puck_oppgoal_distance);    
           
        //tick += 1f;
        
    }

    private void OnTriggerEnter(Collider other){

        if(other.CompareTag("Stick")){
            agent.AgentReward(0.1f, "Stick");
        }
    }
//*************************** CALCULATE REWARD FUNCTIONS PUCK ************************************

    // Try adding method which takes into account the angle of the velocity vector instead?

    // Method to calculate velocity of rigidbody in direction of position, + reward/penalty
    public void CalcReward_RB_to_POS(Vector3 position, Rigidbody rigidbody) { // Primary use puck-player
       
        Vector3 RB_POS_direction = (position - rigidbody.transform.position).normalized;
        //Debug.DrawLine(rigidbody.transform.position, position, Color.yellow, 0f);

        //float v_dir = Vector3.Dot(rigidbody.GetPointVelocity(agent.transform.GetChild(6).transform.position), RB_POS_direction); // velocity of stick used
        float v_dir = Vector3.Dot(rigidbody.velocity, RB_POS_direction);

        float angle = Vector3.Angle(rigidbody.velocity, RB_POS_direction);

        if(v_dir > 1f && angle <= angle_tolerance) { // if angle is 0 it travels in exactly same direction
            agent.AddReward(0.1f); 
            //Debug.Log("v_dir is now: " + (v_dir));
        }
    }

    // TODO: Testing separate methods, one for puck-goal, one for puck-agent
     public void CalcReward_RB_to_POSv2(Vector3 position, Rigidbody rigidbody) { //use for puck-goal
       
        Vector3 RB_POS_direction = (position - rigidbody.transform.position).normalized;
        //Debug.DrawLine(rigidbody.transform.position, position, Color.green, 0f);

        //float v_dir = Vector3.Dot(rigidbody.GetPointVelocity(agent.transform.GetChild(6).transform.position), RB_POS_direction); // velocity of stick
        float v_dir = Vector3.Dot(rigidbody.velocity, RB_POS_direction); //changed to rigidbody.velocity for puck
        
        float angle = Vector3.Angle(rigidbody.velocity, RB_POS_direction);

        if(v_dir > 1f && angle <= angle_tolerance) { 
            Debug.DrawRay(rigidbody.transform.position, rigidbody.velocity, Color.blue, 0f);
            agent.AddReward(0.1f); 
            //Debug.Log("v_dir is now: " + (v_dir));
            //Debug.Log("Positive: " + (v_dir));
        }

        if(v_dir < 0) {
            agent.AddReward(-0.1f);
            //Debug.Log("Negative: " + (v_dir));
        }
    }

    public void CalcReward(float distance)
    {
        float PuckVelocity = GetComponent<Rigidbody>().velocity.magnitude;

        //Debug.Log("Puck Velocity: " + PuckVelocity);

        if (PuckVelocity > 0f){
            float reward = (1f / maxstep) * (1f / (distance * distance));

            //Debug.Log("Reward: " + reward); 

            agent.AgentReward(reward, "Puck");
        }
        
    }



}
