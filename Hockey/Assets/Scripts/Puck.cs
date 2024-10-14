using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{

    [SerializeField] private Transform OwnGoal;
    [SerializeField] private Transform OpponentGoal;

    public AgentMove agent; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //the closer the puck is to the goal, the higher the reward

        //if the puck is closer to the opponent's goal, give a positive reward
        //if the puck is closer to the agent's goal, give a negative reward

        Vector3 GoalPos = OpponentGoal.position;
        Vector3 PuckPos = transform.position;

        float distanceToOpponentGoal = Vector3.Distance(GoalPos, PuckPos);

        //normalize the distance
        distanceToOpponentGoal = distanceToOpponentGoal / 15f;

        CalcReward(distanceToOpponentGoal);
        
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Stick")){
            //Debug.Log("Puck hit the stick");
            agent.AddReward(0.2f);
            //agent.EndEpisode();

        }
    }

    public void CalcReward(float distance)
    {
        float maxstep = 500f; // Define maxstep
        float reward = (1f / maxstep) * (1f / (distance * distance));

        //Debug.Log("Reward: " + reward); 

        agent.AddReward(reward);
    }



}
