using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<int> rolls = new List<int>();

    private PinSetter pinSetter;
    private Ball ball;
    private ScoreDisplay scoreDisplay;

    void Start()
    {
        pinSetter = FindObjectOfType<PinSetter>();
        ball = FindObjectOfType<Ball>();
        scoreDisplay = FindObjectOfType<ScoreDisplay>();
    }

    public void Bowl(int pinFall)
    {
        try
        {
            rolls.Add(pinFall);

            ActionMaster.Action nextAction = ActionMaster.NextAction(rolls);
            pinSetter.PerfromAction(nextAction);            

            ball.Reset();
        } catch
        {
            Debug.LogWarning("Something went wrong in Bowl()");
        }

        try
        {
            scoreDisplay.FillRolls(rolls);
            scoreDisplay.FillFrames(ScoreMaster1.ScoreCumulative(rolls));
        } catch
        {
            Debug.LogWarning("FillRollCard() has failed");
        }
    }

}
