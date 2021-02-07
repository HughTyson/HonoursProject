using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_ : MonoBehaviour
{

    [SerializeField] Config config;


    static GM_ instance_ = null;
    static bool destroyed_ = false;
    Members members = null;


    static public Members Instance
    {
        get
        {
            if (destroyed_)
                return null;
            if(instance_ == null)
            {
                instance_ = FindObjectOfType<GM_>();
                if(instance_ == null)
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("Config/GAME_MANAGER"));
                    obj.name = "Game Manager";
                    instance_ = obj.GetComponent<GM_>();
                }
            }
            return instance_.members;
        }
    }

    //Creates an interface to the GM_ to hide monobehavour methods
    public class Members
    {
        public Config config;
    }

    private void Awake()
    {
        if(instance_ == null)
        {
            DontDestroyOnLoad(gameObject);

            if(instance_ != null && instance_ != this)
            {
                Debug.LogError("Error, multiple GAME MANAGERS!");
                Debug.Break();
            }

            instance_ = this;

            members = new Members();

            members.config = config;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance_ == this)
        {
            for (int i = instance_.gameObject.transform.childCount - 1; i >= 0; i--)
                Destroy(instance_.gameObject.transform.GetChild(i).gameObject);
            instance_ = null;
            destroyed_ = true;
        }
    }
}
