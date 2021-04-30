using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsCanvas : MonoBehaviour
{
    [SerializeField] bool can_hide = false;
    Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(can_hide)
        {
            //hide the canvas then the player hits the return key
            if(Input.GetKey(KeyCode.Return))
            {
                canvas.enabled = false;
            }
        }


    }
}
