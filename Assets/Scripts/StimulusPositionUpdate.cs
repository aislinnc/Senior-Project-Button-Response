using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusPositionUpdate : MonoBehaviour {
/*
    public GameObject eyetracker;
    public GameObject head_camera;
    public GameManager gameManager = GameManager.instance;
    public ViveSR.anipal.Eye.SR_GazeGetter eyetrackerData;

    Quaternion fixation_location;
    Vector3 cameraRaycast;
    Vector3 basePoint;
	Quaternion offsets;
    public Vector3 sizeinUnityUnits;
	
    // Use this for initialization
    void Start() {

		offsets = Quaternion.Euler(Experiment.X_offset, Experiment.Y_offset, 0);
        fixation_location = Quaternion.Euler(gameManager.fixation_location);
        head_camera = GameObject.FindGameObjectWithTag("MainCamera");
        eyetracker = GameObject.FindGameObjectWithTag("Eyetracker");
        eyetrackerData = eyetracker.GetComponent<ViveSR.anipal.Eye.SR_GazeGetter>();
        transform.SetParent(head_camera.transform);
        
        UpdateWithGazePosition();

    }


    /// <summary>
    /// Get the  gaze position and add the desired offset
    /// To be called in update if gaze-contingent stimulus is desired
    /// </summary>
    void UpdateWithGazePosition()
    {
        cameraRaycast = offsets * fixation_location * Vector3.forward;//eyetrackerData.binocularEIHdirection;
        basePoint = eyetrackerData.binocularEIHorigin;
        transform.localPosition = basePoint + cameraRaycast * Stimulus.StimDepth;
        transform.LookAt(Camera.main.transform.TransformPoint(eyetrackerData.binocularEIHorigin));
        transform.localRotation *= Quaternion.Euler(90, 0, 0);
        transform.localScale = new Vector3(2*Mathf.Tan((Stimulus.ApertureRad*Mathf.PI)/180) * Stimulus.StimDepth, 0, 2*Mathf.Tan(Stimulus.ApertureRad * Mathf.PI / 180) * Stimulus.StimDepth);
        Debug.DrawLine(transform.parent.position, transform.position, Color.magenta);
	}

    void Update () { 
		
        //set up size conversions to degrees here
        if (Stimulus.GazeContingent)
        {
            UpdateWithGazePosition();
        }


    }
*/
}
