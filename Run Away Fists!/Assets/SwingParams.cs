using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingParams : MonoBehaviour
{
    public float airSpeed = 0;
    public float timeToAirSpeed = 0;
    public float fallSpeed = 0;
    public float gravityMultiplier = 0;
    public float fastFallSpeed = 0;
    public float fastFallGravityMultiplier = 0;
    public int numAirJumps = 2;

    public void setSwingParams(PlatformerMotor2D platformerMotor2D)
    {
        airSpeed = platformerMotor2D.airSpeed;
        timeToAirSpeed = platformerMotor2D.timeToAirSpeed;
        fallSpeed = platformerMotor2D.fallSpeed;
        gravityMultiplier = platformerMotor2D.gravityMultiplier;
        fastFallSpeed = platformerMotor2D.fastFallSpeed;
        fastFallGravityMultiplier = platformerMotor2D.fastFallGravityMultiplier;
		numAirJumps = platformerMotor2D.numOfAirJumps;
    }
    public void setOther(PlatformerMotor2D platformerMotor2D)
    {
        platformerMotor2D.airSpeed = airSpeed;
        platformerMotor2D.timeToAirSpeed = timeToAirSpeed;
        platformerMotor2D.fallSpeed = fallSpeed;
        platformerMotor2D.gravityMultiplier = gravityMultiplier;
        platformerMotor2D.fastFallSpeed = fastFallSpeed;
        platformerMotor2D.fastFallGravityMultiplier = fastFallGravityMultiplier;
		platformerMotor2D.numOfAirJumps = numAirJumps;
    }
}
