using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLeft : MonoBehaviour
{

    [SerializeField] float max_time = 3;
    [SerializeField] Text text;
    bool start_timer = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.Return))
        {
            start_timer = true; //dont start the timeer until the player has hit enter
        }

        if(start_timer)
        {
            if(max_time > 0)//if the time has not hit 0
            {
                max_time -= Time.deltaTime; //decrease the time

                //display the time in minutes and seconds
                float minutes = Mathf.FloorToInt(max_time / 60);
                float seconds = Mathf.FloorToInt(max_time % 60);

                text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                //the timer has hit, hence as k player to return to the survey
                max_time = 0;

                text.text = "Return to survey";
            }
        }



    }
}
