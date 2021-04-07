using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;


public class DotStimScript : MonoBehaviour {
    // Added by Aislinn to get session    
    public Session dotSession;
    public GameObject experiment;
    public ExperimentGenerator experimentGenerator;
    public CreateDotMotion createDotMotion;
    public int currentLevel;
    /*
    public Session dotSession;
    public GameObject uxf_rig;
    public SessionLogger sessionLogger;
    */
    private List<int> stimLocList;
    private string stimPosSt;
    
    //When the dot stimulus is instantiated, it calls this to create all the dots it needs as children and set them moving.
    public int num_dots;
    public int num_noise;
    public int num_signal;
    public float dot_diam_units;
    public float ap_rad_units;
    public int stim_directionH;
    public int stim_directionV;
    public int stim_direction;
    public string combined_direction;
    public float max_angle;
    public float draw_time;
    public float wait_time;
    public float stim_start_time;
    public float[] DeterministicAngles;

    List<GameObject> dots;

    public void Awake(){
        // Added by Aislinn to get session 
        experiment = GameObject.FindGameObjectWithTag("Experiment");
        experimentGenerator = experiment.GetComponent<ExperimentGenerator>();
        dotSession = experimentGenerator.session;

        // Added by Aislinn to change angle with difficulty
        createDotMotion = experiment.GetComponent<CreateDotMotion>();
        currentLevel = createDotMotion.level;

        /*
        //stim_direction = Random.Range(0, 2);
        if (stim_directionH == 0 && stim_directionV == 0)
        {
            combined_direction = "Northeast";
        }
        if (stim_directionH == 0 && stim_directionV == 1)
        {
            combined_direction = "Southeast";
        }
        if (stim_directionH == 1 && stim_directionV == 0)
        {
            combined_direction = "Northwest";
        }
        if (stim_directionH == 1 && stim_directionV == 1)
        {
            combined_direction = "Southwest";
        }
        */

        // Added by Aislinn to get the dots to go in a tangential direction
        stimLocList = dotSession.settings.GetIntList("stimulusLocation");
        if(stimLocList[1] == 45){
            stimPosSt = "upperRight";
        }
        else if(stimLocList[1] == 135){
            stimPosSt = "lowerRight";
        }
        else if(stimLocList[1] == 225){
            stimPosSt = "lowerLeft";
        }
        else{
            stimPosSt = "upperLeft";
        }

        stim_direction = Random.Range(0,2);

        if(stimPosSt == "upperRight"){
            if(stim_direction == 0){
                combined_direction = "Northwest";
                stim_directionH = 1;
                stim_directionV = 0;
            }
            else{
                combined_direction = "Southeast";
                stim_directionH = 0;
                stim_directionV = 1;
            }
        }
        else if(stimPosSt == "lowerRight"){
            if(stim_direction == 0){
                combined_direction = "Northeast";
                stim_directionH = 0;
                stim_directionV = 0;
            }
            else{
                combined_direction = "Southwest";
                stim_directionH = 1;
                stim_directionV = 1;
            }
        }
        else if(stimPosSt == "lowerLeft"){
            if(stim_direction == 0){
                combined_direction = "Northwest";
                stim_directionH = 1;
                stim_directionV = 0;
            }
            else{
                combined_direction = "Southeast";
                stim_directionH = 0;
                stim_directionV = 1;
            }
        }
        else{
            if(stim_direction == 0){
                combined_direction = "Northeast";
                stim_directionH = 0;
                stim_directionV = 0;
            }
            else{
                combined_direction = "Southwest";
                stim_directionH = 1;
                stim_directionV = 1;
            }
        }
    }
    
    public void Start()
    {       
        //Debug.Log("Stimulus Direction is: " + combined_direction);
        num_dots = Mathf.RoundToInt(Mathf.Pow(dotSession.settings.GetFloat("ApertureRad"), 2f) * Mathf.PI * dotSession.settings.GetFloat("Density"));

        num_noise = (int)(num_dots * dotSession.settings.GetInt("PctNoiseDots") / 100);
        num_signal = (int)(num_dots * (1 - (dotSession.settings.GetInt("PctNoiseDots") / 100)));

        dot_diam_units = ((dotSession.settings.GetInt("DotSize") * Mathf.PI) / (60 * 180)) * dotSession.settings.GetFloat("StimDepth"); //convert arcmin to radians (drop sin term due to small angle approx) and scale by depth
        ap_rad_units = ((dotSession.settings.GetFloat("ApertureRad") * Mathf.PI) / (180)) * dotSession.settings.GetFloat("StimDepth");
        dots = new List<GameObject>();
        stim_start_time = Time.realtimeSinceStartup;
        //Debug.Log("Coherence at " + max_angle);

        if (!dotSession.settings.GetBool("BalanceNoise"))
        {
            for (int i = 0; i < (int)num_dots; i++)
            {
                GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
                dot.name = "Dot" + i.ToString();
                dot.transform.parent = transform;
                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                //Vector2 dot_position = new Vector2(0,0);
                dot.transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                dot.transform.localScale = new Vector3(dot_diam_units / (2 * ap_rad_units), 0, dot_diam_units / (2 * ap_rad_units));
                dot.GetComponent<DotMotion>().current_angle = max_angle;
                if (Random.value <= (dotSession.settings.GetInt("PctNoiseDots") / 100))
                { dot.GetComponent<DotMotion>().isNoise = true; }
                else { dot.GetComponent<DotMotion>().isNoise = false; }

                dot.GetComponent<DotMotion>().current_directionH = stim_directionH;
                dot.GetComponent<DotMotion>().current_directionV = stim_directionV;
                dot.SetActive(false);
                dots.Add(dot);
            }
        }
        if (dotSession.settings.GetBool("BalanceNoise"))
        {
            if (num_noise % 2 != 0)
            {
                num_noise += 1;
                //num_dots = num_noise + num_signal;

               
            }
            num_dots = num_noise + num_signal;
            
            // ADDED BY AISLINN TO SET MAX ANGLE
            max_angle = (currentLevel - 1) * 40;
            if(max_angle > 340){
                max_angle = 340;
            }
            dotSession.CurrentTrial.result["max_angle"] = max_angle;
            
            DeterministicAngles = linspace(-max_angle / 2, max_angle / 2, num_signal);
            for (int i = 0; i < num_noise/2; i++) //Noise Fraction of dots
            {
                GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
                GameObject buddydot = (GameObject)Instantiate(Resources.Load("SingleDot"));

                dot.name = "Dot" + i.ToString();
                dot.GetComponent<DotMotion>().buddyNumber = num_noise - i;
                buddydot.name = "Dot" + (num_noise - i).ToString();
                buddydot.GetComponent<DotMotion>().buddyNumber = i;

                dot.transform.parent = transform;
                buddydot.transform.parent = transform;

                Vector2 dot_position = Random.insideUnitCircle * 0.5f;
                Vector2 buddydot_position = Random.insideUnitCircle * 0.5f;

                dot.transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);
                buddydot.transform.localPosition = new Vector3(buddydot_position[0], 0, buddydot_position[1]);

                dot.transform.localScale = new Vector3(dot_diam_units / (2 * ap_rad_units), 0, dot_diam_units / (2 * ap_rad_units));
                buddydot.transform.localScale = new Vector3(dot_diam_units / (2 * ap_rad_units), 0, dot_diam_units / (2 * ap_rad_units));

                dot.GetComponent<DotMotion>().current_angle = max_angle;
                buddydot.GetComponent<DotMotion>().current_angle = max_angle;

                dot.GetComponent<DotMotion>().isNoise = true;
                buddydot.GetComponent<DotMotion>().isNoise = true;
                buddydot.GetComponent<DotMotion>().isBuddy = true;

                dot.GetComponent<DotMotion>().current_directionH = stim_directionH;
                dot.GetComponent<DotMotion>().current_directionV = stim_directionV;
                buddydot.GetComponent<DotMotion>().current_directionH = stim_directionH;
                buddydot.GetComponent<DotMotion>().current_directionV = stim_directionV;
                dot.SetActive(false);
                buddydot.SetActive(false);

                dots.Add(dot);
                dots.Add(buddydot);
            }
            for ( int i = 0; i < num_signal; i++) //Signal fraction of dots, no buddies
            {

                GameObject dot = (GameObject)Instantiate(Resources.Load("SingleDot"));
              

                dot.name = "SigDot" + i.ToString();

          
                dot.transform.parent = transform;


                Vector2 dot_position = Random.insideUnitCircle * 0.5f;


                dot.transform.localPosition = new Vector3(dot_position[0], 0, dot_position[1]);


                dot.transform.localScale = new Vector3(dot_diam_units / (2 * ap_rad_units), 0, dot_diam_units / (2 * ap_rad_units));


                dot.GetComponent<DotMotion>().current_angle = max_angle;


                dot.GetComponent<DotMotion>().isNoise = false;


                if (stim_directionH == 1)
                {
                    //Debug.Log(i.ToString());
                    dot.GetComponent<DotMotion>().movement_angle = Quaternion.AngleAxis(DeterministicAngles[i], Vector3.up);
                    dot.GetComponent<DotMotion>().pretty_movement_angle = DeterministicAngles[i];

                }
                if (stim_directionH == 0)
                {
                    //Debug.Log(i.ToString());
                    dot.GetComponent<DotMotion>().movement_angle = Quaternion.AngleAxis(-DeterministicAngles[i], Vector3.up);
                    dot.GetComponent<DotMotion>().pretty_movement_angle = DeterministicAngles[i];
                }               

                dot.GetComponent<DotMotion>().current_directionH = stim_directionH;
                dot.GetComponent<DotMotion>().current_directionV = stim_directionV;

                dot.SetActive(false);


                dots.Add(dot);

            }


            

        }
        drawDots();
    }


    private static float[] linspace(float start, float end, int n)
    {
        // Equally spaced linear values between two points
        float[] xs = new float[n];
        xs[0] = start;
        xs[n - 1] = end;
        float delta = (end - start) / (n - 1);
        for (int i = 1; i < n - 1; ++i)
        {
            xs[i] = xs[i - 1] + delta;
        }
        return xs;
    }

    private IEnumerator DrawAfterSeconds(float seconds, GameObject dot)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log("start time is: " + Time.realtimeSinceStartup);
        dot.SetActive(true);
        dot.GetComponent<DotMotion>().start_of_dot = Time.realtimeSinceStartup;
        Debug.Log("recorded time is: " + Time.realtimeSinceStartup);

    }

    void drawDots()
    {
        wait_time = (dotSession.settings.GetFloat("Duration") ) / num_dots;

        for(int i = 0; i < num_dots; i++)
        {
            if (!dots[i].activeInHierarchy)
            {
                dots[i].SetActive(true);
                dots[i].GetComponent<DotMotion>().start_of_dot = Time.realtimeSinceStartup;
                dots[i].GetComponent<DotMotion>().start_of_stimulus = stim_start_time;
            }
        }

    }
}
