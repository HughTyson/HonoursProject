using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockInfoDisplay : MonoBehaviour
{


    [SerializeField] List<GameObject> connected_objs;
    [SerializeField] List<int> connected_numbers;

    public void SetInfo(List<GameObject> objs, List<int> numbers)
    {
        connected_objs = objs;
        connected_numbers = numbers;
    }
}
