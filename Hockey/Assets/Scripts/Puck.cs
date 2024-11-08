using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{

    [SerializeField] private Transform OwnGoal;
    [SerializeField] private Transform OpponentGoal;

    public AgentMove agent; 
    
    private float angle_tolerance_puck = 50f; // tolerance for diff between velocity and direction 
    
    private float angle_tolerance_agent = 25f;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 Stick_pos = agent.transform.GetChild(6).transform.position;
        Vector3 GoalPos = OpponentGoal.position;
        Vector3 PuckPos = transform.position;
        
        CalcReward_RB_to_POSv2(GoalPos, GetComponent<Rigidbody>()); 
        //CalcReward_RB_to_POS(PuckPos, agent.GetComponent<Rigidbody>());


        
        //CalcReward_Puck_to_Goal(PuckPos, agent.GetComponent<Rigidbody>().transform.position);

        //the closer the puck is to the goal, the higher the reward
    
        // Reward based on Velocity of puck in relation to direction of opponentGoal
        
        //normalize the distance
        //distanceToOpponentGoal = distanceToOpponentGoal / 15f;

        //float puck_oppgoal_distance = Vector3.Distance(GoalPos,  PuckPos);

        //CalcReward(puck_oppgoal_distance);    
           
        //tick += 1f;
        
    }

    // TODO: Test effect of training with ontriggerstay as opposed to ontriggerenter
    private void OnTriggerStay(Collider other){ 

        // if(other.CompareTag("Stick")){
        //     agent.AgentReward(1/500000f, "Stick");
        // }
        
        if(other.CompareTag("Wall") && GetComponent<Rigidbody>().velocity.magnitude < 1.3f){ // FIXED: Kan ha 1.29 > velocity > 0 trots den är fast vid vägg
            agent.AgentReward(-1/500000f, "Wall");
            Debug.Log("Fucking walls: " + -1/500000f);
        }
        
    }

    private void OnTriggerEnter(Collider other){

        // if(other.CompareTag("Stick")){
        //     agent.AgentReward(0.1f, "Stick");
        // }
    }
//*************************** CALCULATE REWARD FUNCTIONS PUCK ************************************
    // Method to calculate velocity of rigidbody in direction of position, + reward/penalty
    public void CalcReward_RB_to_POS(Vector3 position, Rigidbody rigidbody) { // Primary use puck-player
       
        Vector3 RB_POS_direction = (position - rigidbody.transform.position).normalized;
        //Debug.DrawLine(rigidbody.transform.position, position, Color.yellow, 0f);

        //float v_dir = Vector3.Dot(rigidbody.GetPointVelocity(agent.transform.GetChild(6).transform.position), RB_POS_direction); // velocity of stick used
        float v_dir = Vector3.Dot(rigidbody.velocity, RB_POS_direction);

        float angle = Vector3.Angle(rigidbody.velocity, RB_POS_direction);

        if(v_dir > 1f && angle <= angle_tolerance_agent) { // if angle is 0 it travels in exactly same direction
            agent.AddReward(1/500000f); 
            //Debug.Log("Nice Angle brate! " + angle);
        }
    }

    // TODO: Testing separate methods, one for puck-goal, one for puck-agent
     public void CalcReward_RB_to_POSv2(Vector3 position, Rigidbody rigidbody) { //use for puck-goal, TODO: USE ANGLE AS PARAMETER??
       
        Vector3 RB_POS_direction = (position - rigidbody.transform.position).normalized;
        //Debug.DrawLine(rigidbody.transform.position, position, Color.green, 0f);

        float v_dir = Vector3.Dot(rigidbody.velocity, RB_POS_direction); //changed to rigidbody.velocity for puck
        
        float angle = Vector3.Angle(rigidbody.velocity, RB_POS_direction);

        if(v_dir > 1f && angle <= angle_tolerance_puck) { 
            //Debug.DrawRay(rigidbody.transform.position, rigidbody.velocity, Color.blue, 0f);
            agent.AddReward(1/500000f); 
        }

        /* if(v_dir < 0 && angle >= 95f) {
            agent.AddReward(-0.1f);
        } */
    }

    public void CalcReward(float distance) {

        float PuckVelocity = GetComponent<Rigidbody>().velocity.magnitude;

        if (PuckVelocity > 0f){
            float reward = (1f / 50000) * (1f / (distance * distance));

            agent.AgentReward(reward, "Puck");
        }
    }
}
