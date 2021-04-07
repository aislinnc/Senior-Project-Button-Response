using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class ExperimentGenerator : MonoBehaviour
{
    public Session session;
    private Block currentBlock;

    // Generate experiment session
    public void Generate(){
        currentBlock = session.CreateBlock();
    }

    // Create and begin the first trial
    public void StartFirstTrial(){
        Trial firstTrial = currentBlock.CreateTrial();
        firstTrial.Begin(); 
    }
}
