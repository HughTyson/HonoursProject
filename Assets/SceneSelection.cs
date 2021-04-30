using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSelection : MonoBehaviour
{

    [SerializeField] List<string> scene_names;
    int scene_selection;
    bool ensure_render = false;
    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Random.InitState((int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds); //set a random seed
        scene_selection = Random.Range(0, scene_names.Count); //get one of the scenes
        
    }

    private void Update()
    {
        if(ensure_render)
            SceneManager.LoadScene(scene_names[scene_selection]); //load the new scene

        ensure_render = true;
    }

}
