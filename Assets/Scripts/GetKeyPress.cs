  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class GetKeyPress : MonoBehaviour
{
    public Session session;
    public GameObject target;
    public GameObject distractor;
    private CreateDotMotion createDotMotion;
    public bool keyPressed;
    public string combined_direction;
    public bool keySuccess;
    private int currentLevel;
    public int nextLevel;
    private int maxLevel;
    private bool lastTrial;
    

    public void Start(){
        // Access the createDotMotionScript
        createDotMotion = GetComponent<CreateDotMotion>();
        lastTrial = false;

        /*
        // Get the distractor and targets trackers
        targetController = target.GetComponent<TargetController>();
        distractorController = distractor.GetComponent<DistractorController>();
        */
    }

    // Update is called once per frame
    public void Update()
    {   
        // Doesn't register key presses until stimulus is gone
        if(createDotMotion.stimActive == false){
            // Get the combined direction of the dot motion
            combined_direction = createDotMotion.combined_direction;
            
            // If the left arrow key was pressed 
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                // If it was the right key to press
                if(combined_direction == "Northwest" || combined_direction == "Southwest"){
                    keySuccess = true;
                    EndTrial();
                }
                // If it was the wrong key
                else{
                    keySuccess = false;
                    EndTrial();
                }
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow)){
                if(combined_direction == "Northeast" || combined_direction == "Southeast"){
                    keySuccess = true;
                    EndTrial();
                }
                else{
                    keySuccess = false;
                    EndTrial();
                }
            }
        }
    }

    public void EndTrial(){
        /*
        // Stop target and distractor controller
        targetController.targetTracker.StopRecording();
        distractorController.distractorTracker.StopRecording();
        */

        if(keySuccess == true){
            session.CurrentTrial.result["outcome"] = "success";
            Debug.Log("trial success");
        }
        else{
            session.CurrentTrial.result["outcome"] = "fail";
            Debug.Log("trial fail");
        }

        DifficultyAdjuster();

        if(lastTrial == false){
            // Destroy target and distractor at the end of the trail
            target = GameObject.FindGameObjectWithTag("Target");
            distractor = GameObject.FindGameObjectWithTag("Distractor");
            Destroy(target);
            Destroy(distractor);

            // End the current trial and start the next
            Block currentBlock = session.CurrentBlock;
            Trial newTrial = currentBlock.CreateTrial();
            session.EndCurrentTrial();
            newTrial.Begin();
        }
        else{
            // Destroy target and distractor at the end of the trail
            target = GameObject.FindGameObjectWithTag("Target");
            distractor = GameObject.FindGameObjectWithTag("Distractor");
            Destroy(target);
            Destroy(distractor);

            // End the current trial
            Block currentBlock = session.CurrentBlock;
            Trial newTrial = currentBlock.CreateTrial();
            session.EndCurrentTrial();
                
            // End the session
            session.End();
        }
    }

    public void DifficultyAdjuster(){   
        // Get the current number of trials 
        int numTrials = session.CurrentTrial.numberInBlock;

        // Get the current trial level
        currentLevel = (int) session.GetTrial(numTrials).result["difficulty_level"];

        // Get the max level 
        maxLevel = session.settings.GetInt("difficultyLevels");

        // If there are less than 3 past levels, the next level will be 1
        if(numTrials < 3){
            nextLevel = 1;
        }
        // 3 or more trials
        else{
            //Debug.Log("ATTENTION: More than 3 trials");
            // Check if the current trial was a success
            if((string) session.GetTrial(numTrials).result["outcome"] == "success"){    
                // Check if the past 2 trials were the same level and if they were successful
                bool sameLevel = true;
                bool successful = true;
                for(int i = 1; i < 3; i++){
                    int pastTrialLevel = (int) session.GetTrial(numTrials-i).result["difficulty_level"];
                    if(pastTrialLevel != currentLevel){
                        sameLevel = false;
                    }
                    string pastTrialSuccess = (string) session.GetTrial(numTrials-i).result["outcome"];
                    if (pastTrialSuccess == "fail"){
                        successful = false;
                    }
                }
                // If both are true the level increases, if not it remains the same
                if(sameLevel == true && successful == true){
                    // Check if it's the last level
                    if(currentLevel == maxLevel){
                        lastTrial = true;
                    }
                    else{
                        lastTrial = false;
                        nextLevel = currentLevel + 1;
                    }
                }
                else{
                    nextLevel = currentLevel;
                }
            }
            // If the current trial was unsuccesful
            else{
                if(currentLevel > 1){
                    nextLevel = currentLevel - 1;
                }
                else{
                    nextLevel = currentLevel;
                }
            }        
        }
    }
}
