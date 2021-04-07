using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class DistractorController : MonoBehaviour
{
    public Session distSession;
    public GameObject experiment;
    public ExperimentGenerator experimentGenerator;
    // Movement
    public CreateDotMotion createDotMotion;
    private string combinedDirection;
    private int moveSpeed;
    private Vector3 distDir;
    private string distDirection;
    //public Tracker distractorTracker;

    void Start(){
        // Get session
        experiment = GameObject.FindGameObjectWithTag("Experiment");
        experimentGenerator = experiment.GetComponent<ExperimentGenerator>();
        distSession = experimentGenerator.session;
 
        // Get the direction of the stimulus dots and go the opposite way
        createDotMotion = experiment.GetComponent<CreateDotMotion>();
        combinedDirection = createDotMotion.combined_direction;

        // Set the direction and speed the distractor
        moveSpeed = distSession.settings.GetInt("distractorMoveSpeed");
        transform.Rotate(90f, 0f, 0f);
        if(combinedDirection == "Northeast"){
            distDir = new Vector3(-1f, -1f, 0f);
            distDirection = "Southwest";
        }
        else if(combinedDirection == "Southeast"){
            distDir = new Vector3(-1f, 1f, 0f);
            distDirection = "Northwest";
        }
        else if(combinedDirection == "Northwest"){
            distDir = new Vector3(1f, -1f, 0f);
            distDirection = "Southeast";
        }
        else{
            distDir = new Vector3(1f, 1f, 0f);
            distDirection = "Northeast";
        }

        // Log distractor direction
        distSession.CurrentTrial.result["distractor_direction"] = distDirection; 

        /*
        // Add a tracker of its position
        distSession.trackedObjects.Add(distractorTracker);
        distractorTracker.StartRecording();
        */
    }

    void Update()
    {
        // Send the distractor in the opposite direction of the target
        transform.Translate(distDir*moveSpeed*Time.deltaTime, Space.World);
    }
}
