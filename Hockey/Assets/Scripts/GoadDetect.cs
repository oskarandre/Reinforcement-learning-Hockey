using UnityEngine;

public class GoalDetectWithInput : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public AgentMove agent; 

    void OnCollisionEnter(Collision col)
    {
        // Touched goal.
        if (col.gameObject.CompareTag("OpponentGoal"))
        {
            agent.ScoredAGoal(1f);
            //Debug.Log("Scored");
        }
        else if (col.gameObject.CompareTag("OwnGoal"))
        {
            agent.ScoredAGoal(-1f);
            //Debug.Log("Own goal u fukking loser");
        }
    }
}