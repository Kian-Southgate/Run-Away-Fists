using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Swing : MonoBehaviour
{
    bool inSwing = false;
	SwingParams previous = new SwingParams();
	public SwingParams inAir;
    public void enterSwing(PlatformerMotor2D platformerMotor2D)
    {
        if (inSwing == false)
        {
            inSwing = true;
            previous.setSwingParams(platformerMotor2D);
			inAir.setOther(platformerMotor2D);
        }
        
    }
    public void exitSwing(PlatformerMotor2D platformerMotor2D)
    {
        if (inSwing)
        {
            inSwing = false;
            previous.setOther(platformerMotor2D);
		}
    }
}
