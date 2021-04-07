using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class TargetController : MonoBehaviour
{
    public Session targetSession;
    public GameObject experiment;
    public ExperimentGenerator experimentGenerator;
    // Movement
    public CreateDotMotion createDotMotion;
    private string combinedDirection;
    private int moveSpeed;
    public Vector3 targetDir;
    //public Tracker targetTracker;
    public Renderer rend;

    void Start(){
        // Get session
        experiment = GameObject.FindGameObjectWithTag("Experiment");
        experimentGenerator = experiment.GetComponent<ExperimentGenerator>();
        targetSession = experimentGenerator.session;
 
        // Get the direction of the stimulus dots
        createDotMotion = experiment.GetComponent<CreateDotMotion>();
        combinedDirection = createDotMotion.combined_direction;
        targetSession.CurrentTrial.result["target_direction"] = combinedDirection; 

        // Set the direction and speed the target
        moveSpeed = targetSession.settings.GetInt("targetMoveSpeed");
        transform.Rotate(90f, 0f, 0f);
        if(combinedDirection == "Northeast"){
            targetDir = new Vector3(1f, 1f, 0f);
        }
        else if(combinedDirection == "Southeast"){
            targetDir = new Vector3(1f, -1f, 0f);
        }
        else if(combinedDirection == "Northwest"){
            targetDir = new Vector3(-1f, 1f, 0f);
        }
        else{
            targetDir = new Vector3(-1f, -1f, 0f);
        }

        /*
        // Add a tracker of its position
        targetSession.trackedObjects.Add(targetTracker);
        targetTracker.StartRecording();
        */
    }

    void Update()
    {
        // Target moves in the direction of the stimulus dots 
        transform.Translate(targetDir*moveSpeed*Time.deltaTime,Space.World);
    }
}
