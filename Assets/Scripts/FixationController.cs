using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class FixationController : MonoBehaviour
{   
    public Session session;
    private List<int> fixPosList;
    private Quaternion fixationLoc;
    private float stimDepth;
    
    
    void Start(){
        fixPosList = session.settings.GetIntList("fixationLocation");
        fixationLoc = Quaternion.Euler(fixPosList[0], fixPosList[1], fixPosList[2]);
        stimDepth = session.settings.GetFloat("StimDepth");
    }

    // Activates the fixation point for 1 second then deactivates it 
    IEnumerator Fixate(){
        yield return new WaitForSeconds(session.settings.GetFloat("fixationTime"));
        gameObject.SetActive(false);
    }

    // Starts the Fixate coroutine
    public void StartFixate(){
        StartCoroutine(Fixate());
    }

    void Update(){
        if(gameObject.activeInHierarchy == true){
            transform.localPosition = fixationLoc * Vector3.forward * stimDepth; 
        }
    }
}
