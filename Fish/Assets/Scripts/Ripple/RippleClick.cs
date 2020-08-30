using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleClick : MonoBehaviour
{
    public Ripple ripple;
    public bool clickRipple=false;
    // Update is called once per frame


    void Update()
    {
        rippleCilck();

    }

    public void rippleCilck()
    {
        if (Input.GetMouseButtonDown(1))
        {

            Vector2 mousePos = Input.mousePosition;
            ripple.Emit(mousePos);
        }
        
    }

}
