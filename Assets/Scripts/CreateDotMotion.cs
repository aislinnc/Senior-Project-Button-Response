using System.Collections;
  using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CreateDotMotion : MonoBehaviour
{
    public Session session;
    public GameObject stim;
    public GameObject instStim;
    public DotStimScript dotStimScript;
    private List<float> posList;
    private Vector3 pos;
    public string combined_direction;
    public bool stimActive;
    public GameObject experiment;
    public GetKeyPress getKeyPress;
    public int level;
    public GameObject headCamera;
    private List<int> stimLocList;
    private Quaternion stimLoc;
    private float stimDepth;
    public GameObject fixation;

    public void Start(){
        stimActive = true;

        // For initial location
        headCamera = GameObject.FindGameObjectWithTag("Camera");
        stimLocList = session.settings.GetIntList("stimulusLocation");
        stimLoc = Quaternion.Euler(stimLocList[0], stimLocList[1], stimLocList[2]);
        stimDepth = session.settings.GetFloat("StimDepth");

        // Get the fixation game object
        fixation = GameObject.FindGameObjectWithTag("Fixation");
    }
    
    public void CreateDotMotionStimulus(){
        // Set the location of the stimulus
        posList = session.settings.GetFloatList("stimulusLocation");
        pos =  new Vector3(posList[0], posList[1], posList[2]);

        // Check if it's the first trial 
        int numTrials = session.CurrentTrial.numberInBlock;
        if(numTrials < 2){
            level = 1;
        }
        else{
            experiment = GameObject.FindGameObjectWithTag("Experiment");
            getKeyPress = experiment.GetComponent<GetKeyPress>();
            level = getKeyPress.nextLevel;
        }
        session.CurrentTrial.result["difficulty_level"] = level;

        StartCoroutine(DotMotionCoroutine());
    }

    IEnumerator DotMotionCoroutine(){
        // Wait for the fixation point to dissapear
        yield return new WaitForSeconds(session.settings.GetFloat("fixationTime"));
        
        // Instantiate dot motion stimulus
        // set it to the location of the fixation to begin with 
        instStim = Instantiate(stim, fixation.transform.position, Quaternion.identity);
        // transform.lookat may be what i need
        instStim.transform.Rotate(90f, 180f, 0f);
        instStim.SetActive(true);
        dotStimScript = instStim.GetComponent<DotStimScript>();
        combined_direction = dotStimScript.combined_direction;

        // Display it for .5 seconds
        yield return new WaitForSeconds(session.settings.GetFloat("stimulusTime"));
        
        // Destroy it
        Destroy(instStim);
        stimActive = false;
    }
}
