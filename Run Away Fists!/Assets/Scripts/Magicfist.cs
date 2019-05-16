
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magicfist : MonoBehaviour
{
	Vector3 offset = new Vector3(1.5f, 0.0f, 0.0f);
	public int maxDistance = 3;
	public int maxDistanceForSnap = 4;
	public float launchSpeed = 0.5F;
	public float pullSpeed = 0.5f;
	public float grappelSpeed = 0.5f;
	public LayerMask staticEnvLayerMask;
	public Collider2D bodyCollider;
	public GameObject body;
	public Rigidbody2D bodyRigidBody;
	public Rigidbody2D anchor;
	public PlatformerMotor2D platformerMotor2D;
	public Swing swing;
	float victimPreviousGravity = 0;
	float victimPreviousAngularDrag = 0;
	GameObject victim;
	Collider2D victimCollider;
	Vector3 targetPos;
	DistanceJoint2D constrainer;
	
	public float tolerance = 0.5f;
	FIST_DIRECTION fistDirection = FIST_DIRECTION.NONE;
	enum VICTIM_HELD {STATIC_VICTIM, DYNAMIC_VICTIM, NONE}

	enum FIST_DIRECTION {PULLING,LAUNCHING,GRAPPELING, NONE};
	Vector3 StartPosition()
	{
		return new Vector3(body.transform.localPosition.x + offset.x, body.transform.localPosition.y + offset.y, body.transform.localPosition.z + offset.z);
	}
	VICTIM_HELD victimHeld()
	{
		if (victim == null)
		{
			return VICTIM_HELD.NONE;
		}
		Rigidbody2D otherBody = victim.gameObject.GetComponent<Rigidbody2D>();
		if (otherBody.bodyType == RigidbodyType2D.Static)
		{
			return VICTIM_HELD.STATIC_VICTIM;
		} 
		else 
		{
			return VICTIM_HELD.DYNAMIC_VICTIM;
		}
	}
	Rigidbody2D victimRigidBody ()
	{
		return victim.GetComponent<Rigidbody2D>();
	}
	void Start()
	{
		offset = new Vector3(transform.localPosition.x - body.transform.localPosition.x, transform.localPosition.y - body.transform.localPosition.y, transform.localPosition.z - body.transform.localPosition.z);
		platformerMotor2D.onAirJump += detatchFromStaticVictimAndPull;
		
		bodyRigidBody = body.gameObject.AddComponent<Rigidbody2D>();
		bodyRigidBody.gravityScale = 0;
		bodyRigidBody.bodyType = RigidbodyType2D.Dynamic;
		bodyRigidBody.freezeRotation = true;
		bodyRigidBody.mass = 0f;
		bodyRigidBody.drag = 999;
	
	}
	void FixedUpdate()
	{
		handleInput();
		updateFist();
		updateVictim();
	}

	void handleInput()
	{
		if (Input.GetAxis("Launch Fist") > 0 && fistDirection == FIST_DIRECTION.NONE)
		{
			if (fistNearBody())
			{
				targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				targetPos.z = transform.position.z;
				fistDirection = FIST_DIRECTION.LAUNCHING;
			}
		}
		else if (Input.GetAxis("Pull Fist") > 0 && fistDirection == FIST_DIRECTION.NONE)
		{
			fistDirection = FIST_DIRECTION.PULLING;
			switch(victimHeld())
			{
				case VICTIM_HELD.STATIC_VICTIM:
				{
					detatchVictim();
					constrainer.maxDistanceOnly = false;
					break;
				}
				case VICTIM_HELD.DYNAMIC_VICTIM:
				{
					constrainer.maxDistanceOnly = false;
					break;
				}
				case VICTIM_HELD.NONE:
				{
					break;
				}
			}
		}

		else if (Input.GetAxis("Grappel") > 0 && victimHeld() != VICTIM_HELD.NONE)
		{
			fistDirection = FIST_DIRECTION.GRAPPELING;
		}
	}

	void updateFist()
	{
		switch (fistDirection)
		{
			case FIST_DIRECTION.PULLING:
				{
					pullFist(pullSpeed);

					if (fistNearBody())
					{
						detatchVictim();
						fistDirection = FIST_DIRECTION.NONE;
					}
					break;
				}
			case FIST_DIRECTION.LAUNCHING:
				{
					transform.position = Vector3.MoveTowards(transform.position, targetPos, launchSpeed);
					if (getMagnitude() > maxDistance | Vector3.Distance(transform.position, targetPos) == 0)
					{
						fistDirection = FIST_DIRECTION.PULLING;
					}
					break;
				}
			case FIST_DIRECTION.GRAPPELING: 
			{
				if (fistNearBody())
				{
					detatchVictim();
					fistDirection = FIST_DIRECTION.NONE;
				} else if (victimHeld() != VICTIM_HELD.NONE)
				{
					float previousGravity = platformerMotor2D.gravityMultiplier;
					platformerMotor2D.gravityMultiplier = 0;
					grappelToFist(grappelSpeed);
					platformerMotor2D.gravityMultiplier = previousGravity;
				}
				
				break;
			}
			case FIST_DIRECTION.NONE:
				{
					if (fistNearBody())
					{
						detatchVictim();
					}
					if (victimHeld() == VICTIM_HELD.NONE)
					{
						transform.localPosition = StartPosition();
						fistDirection = FIST_DIRECTION.NONE;
					}
					break;
				}
		}
	}
	void updateVictim()
	{
		switch (victimHeld())
		{
			case VICTIM_HELD.STATIC_VICTIM:
				{
					if (platformerMotor2D.IsInAir())
					{
						swing.enterSwing(platformerMotor2D);
					}
					else
					{
						swing.exitSwing(platformerMotor2D);
					}
					break;
				}
			case VICTIM_HELD.DYNAMIC_VICTIM:
				{
					this.transform.position = victim.transform.position;
					if (getMagnitude() > maxDistanceForSnap)
					{
						detatchVictim();
						fistDirection = FIST_DIRECTION.PULLING;
						constrainer.maxDistanceOnly = false;
					}
					break;
				}
			case VICTIM_HELD.NONE:
				{
					break;
				}
		}
	}
	void detatchFromStaticVictimAndPull()
	{
		if (victimHeld() == VICTIM_HELD.STATIC_VICTIM)
		{
			detatchVictim();
			fistDirection = FIST_DIRECTION.PULLING;
			swing.exitSwing(platformerMotor2D);
		}
	}
	float getMagnitude() 
	{
		return Vector3.Distance(StartPosition(), transform.localPosition);
	}
	void pullFist(float speed)
	{
		switch(victimHeld())
		{
			case VICTIM_HELD.DYNAMIC_VICTIM:
			{
				constrainer.distance -= speed;
				break;
			}
			case VICTIM_HELD.STATIC_VICTIM:
			{
				grappelToFist(speed);
				break;
			}
			case VICTIM_HELD.NONE:
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, StartPosition(), speed);
				break;
			}
		}
	}
    void grappelToFist(float speed)
	{
		body.transform.localPosition = Vector3.MoveTowards(body.transform.localPosition, transform.localPosition, speed);
	}
	void grabVictim(Collider2D col)
	{
		this.victimCollider = col;
		this.victim = col.gameObject;
		
		this.transform.position = victim.transform.position;
		fistDirection = FIST_DIRECTION.NONE;
		Physics2D.IgnoreCollision(victimCollider, bodyCollider, true);

		victimRigidBody().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		victimPreviousGravity = victimRigidBody().gravityScale;
		victimPreviousAngularDrag = victimRigidBody().angularDrag;
		victim.transform.rotation = Quaternion.identity;

		if (victimHeld() == VICTIM_HELD.DYNAMIC_VICTIM)
		{
			constrainer = col.gameObject.AddComponent<DistanceJoint2D>();
			constrainer.connectedBody = anchor;
		} else if (victimHeld() == VICTIM_HELD.STATIC_VICTIM)
		{
			bodyRigidBody.bodyType = RigidbodyType2D.Dynamic;
			bodyRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

			constrainer = body.gameObject.AddComponent<DistanceJoint2D>();
			constrainer.connectedBody = victimRigidBody();
			
		}
		
		constrainer.maxDistanceOnly = true;
		constrainer.distance = maxDistance;
		constrainer.autoConfigureDistance = false;
		anchor.gameObject.transform.localPosition = new Vector3(0, 0, 0);
	}
	void detatchVictim()
	{
		if (victimHeld() != VICTIM_HELD.NONE)
		{
			Destroy(constrainer);
			victimRigidBody().gravityScale = victimPreviousGravity;
			victimRigidBody().angularDrag = victimPreviousAngularDrag;
			victimRigidBody().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
			victimRigidBody().constraints = RigidbodyConstraints2D.None;
			victimRigidBody().angularVelocity = 0;

			Physics2D.IgnoreCollision(victimCollider, bodyCollider, false);
			anchor.gameObject.transform.localPosition = new Vector3(0,0,0);
			victim = null;

			swing.exitSwing(platformerMotor2D);
		}
	}
	bool fistNearBody()
	{
		if (victimHeld() != VICTIM_HELD.NONE)
		{
			return victimCollider.bounds.Intersects(bodyCollider.bounds);
		}
		return Vector3.Distance(transform.localPosition,  StartPosition()) < tolerance;
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (fistDirection == FIST_DIRECTION.LAUNCHING && victimHeld() == VICTIM_HELD.NONE)
		{
			if (((0x1 << col.gameObject.layer) & staticEnvLayerMask) != 0)
			{
				fistDirection = FIST_DIRECTION.PULLING;
			}
			else if (validVictim(col.gameObject))
			{
				grabVictim(col);
			}
		}
	}
	bool validVictim(GameObject other)
	{
		if (other == this)
		{
			return false;
		}
		if (other == body)
		{
			return false;
		}

		Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();
		if (otherBody == null)
		{
			return false;
		}
		
		return true;
	}
}
