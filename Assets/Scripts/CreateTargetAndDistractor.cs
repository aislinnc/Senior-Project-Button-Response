using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CreateTargetAndDistractor : MonoBehaviour
{
    public Session session;
    public GameObject targetPrefab;
    public GameObject distractorPrefab;
    public GameObject parent;
    public GameObject target;
    public GameObject distractor;
    private List<float> posList;
    private Vector3 pos;
    private float waitTime;

    public void CreateTargetAndDistractorObjects()
    {
        // Set postition to position of dot motion
        posList = session.settings.GetFloatList("stimulusLocation");
        pos =  new Vector3(posList[0], posList[1], posList[2]);

        // How long it takes for fixation and dot motion to dissapear
        waitTime = session.settings.GetFloat("fixationTime") + session.settings.GetFloat("stimulusTime");

        StartCoroutine(TargetAndDistractorCoroutine());
    }

    IEnumerator TargetAndDistractorCoroutine(){
        // Wait for dot motion to dissapear 
        yield return new WaitForSeconds(waitTime);

        // Instantiate target and distractor prefavs
        Instantiate(targetPrefab, pos, Quaternion.identity);
        Instantiate(distractorPrefab, pos, Quaternion.identity);
    }
}
