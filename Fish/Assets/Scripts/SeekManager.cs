using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekManager : MonoBehaviour
{
    private FishA[] fish;
    private bool flee;

    // Start is called before the first frame update
    void Start()
    {
        fish = FindObjectsOfType<FishA>();
        flee = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (FishA f in fish)
                f.flee = flee;

            flee = !flee;
        }
    }
}
