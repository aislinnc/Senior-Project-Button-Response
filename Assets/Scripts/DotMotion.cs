using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class DotMotion : MonoBehaviour {
    public float current_angle; //assigned by parent, chosen randomly from list in JSON
    public float current_directionH; //assigned by parent, chosen randomly as 0 or 1 (east, west)
    public float current_directionV; // randomly chosen so that 0 is North and 1 is South
    //public Vector3 destination;
    public Quaternion movement_angle;
    public float pretty_movement_angle;
    public Vector3 movement_end;
    public float speedApertureUnits;
    public float start_of_dot;
    public float start_of_stimulus;
    public float timeSinceStartup;
    public bool isNoise;
    public bool isBuddy;
    public int buddyNumber;
    public GameObject buddyDot;
	// Use this for initialization

    // Added by Aislinn to get session
    public Session dotSession;
    public GameObject experiment;
    public ExperimentGenerator experimentGenerator;
    // Added by Aislinn to get stim_start_time from DotStimScript
    public GameObject dotStimulus;
    public DotStimScript dotStimScript;
    // Added by Aislinn to set location
    public GameObject headCamera;
    private List<int> stimLocList;
    private Quaternion stimLoc;
    private float stimDepth;
    
    
	void Start () 
    {
        // Added by Aislinn to get session
        experiment = GameObject.FindGameObjectWithTag("Experiment");
        experimentGenerator = experiment.GetComponent<ExperimentGenerator>();
        dotSession = experimentGenerator.session;
        
        // Added by Aislinn to get stim_start_time from DotStimScript
        dotStimulus = GameObject.FindGameObjectWithTag("DotStimulus");
        dotStimScript = dotStimulus.GetComponent<DotStimScript>();

        // Added by Aislinn to put stimulus in right location
        headCamera = GameObject.FindGameObjectWithTag("Camera");
        transform.SetParent(headCamera.transform);
        stimLocList = dotSession.settings.GetIntList("stimulusLocation");
        stimLoc = Quaternion.Euler(stimLocList[0], stimLocList[1], stimLocList[2]);
        stimDepth = dotSession.settings.GetFloat("StimDepth");

        speedApertureUnits = dotSession.settings.GetInt("DotSpeed")/(2*dotSession.settings.GetFloat("ApertureRad"));// Aperture has scaled radius of 1 here, need to scale speed to use with local position

        if (current_directionH == 1 && !isNoise) // "right opening" movement wedge (use for motion with leftward component)
        {
            if (!dotSession.settings.GetBool("DeterministicSignal")) { movement_angle = Quaternion.AngleAxis(-Random.Range(-current_angle / 2, current_angle / 2), Vector3.up); }
            
            transform.localRotation = Quaternion.identity * movement_angle;
            //Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        if( current_directionH != 1 && ! isNoise) // "left opening" movement wedge (use for motion with rightward component)
        {
            if (!dotSession.settings.GetBool("DeterministicSignal"))
            { movement_angle = Quaternion.AngleAxis(Random.Range(-current_angle / 2, current_angle / 2), Vector3.up); }
            transform.localRotation = Quaternion.identity * movement_angle;
           // Debug.Log("movement angle is " + movement_angle.eulerAngles);
        }
        if (isNoise && !isBuddy) 
        {
            buddyDot = GameObject.Find("Dot" + buddyNumber.ToString());
            float angle_degrees = Random.Range(-360, 360);
            movement_angle = Quaternion.AngleAxis(angle_degrees, Vector3.up);
            buddyDot.GetComponent<DotMotion>().movement_angle = Quaternion.AngleAxis(angle_degrees+180, Vector3.up);
            transform.localRotation = Quaternion.identity * movement_angle;
        }
        if (isNoise && isBuddy)
        {
            transform.localRotation = Quaternion.identity * movement_angle;
        }
        //GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        
        timeSinceStartup = Time.realtimeSinceStartup;
    }



	void Update () {
        // Added by Aislinn to update position
        transform.localPosition = stimLoc * Vector3.forward * stimDepth;

        start_of_stimulus = dotStimScript.stim_start_time; // AISLINN ADDED 

        if ((Time.realtimeSinceStartup) < dotSession.settings.GetFloat("Duration") + start_of_stimulus)
        {
            gameObject.SetActive(true);
            
            if ((Time.realtimeSinceStartup) < dotSession.settings.GetFloat("DotLife") + start_of_dot)
            {
                //float deltaT = 1 / 90f; //change this after debugging
                if (current_directionH == 1 && current_directionV ==1)
                {
                    transform.Translate((Vector3.forward+Vector3.right) * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);
                }
                if (current_directionH == 1 && current_directionV == 0)
                {
                    transform.Translate((-Vector3.forward + Vector3.right) * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);
                }
                if (current_directionH == 0 && current_directionV == 0)
                {
                    transform.Translate((-Vector3.forward - Vector3.right) * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);
                }
                if (current_directionH == 0 && current_directionV == 1)
                {
                    transform.Translate((Vector3.forward - Vector3.right) * speedApertureUnits * Time.deltaTime * transform.parent.localScale.x, Space.Self);
                }

                if (transform.localPosition.magnitude >= 0.5f)
                {
                    float old_x = transform.localPosition.x;
                    float old_z = transform.localPosition.z;
                    gameObject.SetActive(false);
                    transform.localPosition = new Vector3(-old_x, 0, -old_z);
                    gameObject.SetActive(true);
                }
            }
            else
            {
               // Debug.Log("Respawning dot");
                gameObject.SetActive(false);
                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                start_of_dot = Time.realtimeSinceStartup;
                gameObject.SetActive(true);
            }
            
        }
        else { gameObject.SetActive(false); }

        //if (Time.realtimeSinceStartup > start_of_stimulus + 9f)
        //{
        //    Debug.Log("Dot is "+ transform.localPosition.magnitude +" away from center");
        //    Debug.Break();
        //}
	}
}
