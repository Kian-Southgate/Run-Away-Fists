
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
	public float grappelSpeed = 0.25f;
	public LayerMask staticEnvLayerMask;
	public Rigidbody2D rigidbody2D;
	public Collider2D collider;
	public Collider2D bodyCollider;
	public GameObject body;
	public PlatformerMotor2D platformerMotor2D;
	public Swing swing;
	float victimPreviousGravity = 0;
	float victimPreviousAngularDrag = 0;
	float previousGravity = 0;
	GameObject victim;
	Collider2D victimCollider;
	Vector3 targetPos;
	public float tolerance = 0.5f;
	public bool offbody = false;
	public bool pulling = false;
	Vector3 StartPosition()
	{
		return new Vector3(body.transform.localPosition.x + offset.x, body.transform.localPosition.y + offset.y, body.transform.localPosition.z + offset.z);
	}
	Rigidbody2D victimRigidBody ()
	{
		if (victim == null)
		{
			return null;
		} else 
		{
			return victim.GetComponent<Rigidbody2D>();
		}
	}
	void Start()
	{
		offset = new Vector3(transform.localPosition.x - body.transform.localPosition.x, transform.localPosition.y - body.transform.localPosition.y, transform.localPosition.z - body.transform.localPosition.z);
		previousGravity = platformerMotor2D.gravityMultiplier;
		platformerMotor2D.onJump += detatchFromStaticVictimAndPull;
	}
	
	

	void FixedUpdate()
	{
		handleInput();

		if (pulling)
		{
			pullFist(pullSpeed);
		}

		if (nearStart() && fistAttatched() == false)
		{
			resetFist();
		}

		if (victim != null)
		{
			if (victimIsStatic())
			{
				if (platformerMotor2D.IsInAir())
				{
					swing.enterSwing(platformerMotor2D);
				}
				else
				{
					swing.exitSwing(platformerMotor2D);
				}
			} 
			else 
			{
				this.transform.position = victim.transform.position;
			}
		}

		constrainDistanceToBody();

		if (offbody == false)
		{
			transform.localPosition = StartPosition();
			pulling = false;
		}
	}

	void handleInput()
	{
		if (Input.GetAxis("Launch Fist") > 0)
		{
			if (fistAttatched())
			{
				if (nearStart())
				{
					targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					targetPos.z = transform.position.z;
					transform.position = Vector3.MoveTowards(transform.position, targetPos, launchSpeed);
					offbody = true;
				}
				else
				{
					transform.position = Vector3.MoveTowards(transform.position, targetPos, launchSpeed);
				}
			}
		}
		if (Input.GetAxis("Launch Fist") == 0 && nearStart() == false && victim == null)
		{
			finishLaunchFist();
		}
		
		if (Input.GetAxis("Pull Fist") > 0)
		{
			if (victimIsStatic())
			{
				detatchVictim();
			}
			pulling = true;
		}

		if (Input.GetAxis("Grappel") > 0 && victim != null)
		{
			platformerMotor2D.gravityMultiplier = 0;
			grappelToFist(grappelSpeed * 2);
		}

		if (Input.GetAxis("Grappel") == 0 | victim == null)
		{
			platformerMotor2D.gravityMultiplier = previousGravity;
		}
	}

	void detatchFromStaticVictimAndPull()
	{
		if (victimIsStatic())
		{
			detatchVictim();
			pulling = true;
		}
	}
	bool fistAttatched()
	{
		return rigidbody2D.bodyType == RigidbodyType2D.Kinematic;
	}

	void finishLaunchFist()
	{
		rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
		collider.isTrigger = false;
	}
	float getMagnitude() 
	{
		return Vector3.Distance(StartPosition(), transform.localPosition);
	}
	void pullFist(float speed)
	{
		if (victim == null)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, StartPosition(), speed);
		} 
		else 
		{
			Vector2 change = Vector2.MoveTowards(victim.transform.position, body.transform.position, speed);
			victimRigidBody().MovePosition(change);
		}
	}

    
    bool victimIsStatic()
    {
        if (victim == null)
        {
			return false;
		} else 
		{
            Rigidbody2D otherBody = victim.gameObject.GetComponent<Rigidbody2D>();
            if (otherBody.bodyType == RigidbodyType2D.Static)
            {
                return true;
            } 
			else 
			{
				return false;
			}
        }
    }
    void grappelToFist(float speed)
	{
		body.transform.localPosition = Vector3.MoveTowards(body.transform.localPosition, transform.localPosition, speed);
	}
	public void resetFist()
	{
		rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
		rigidbody2D.velocity = new Vector2(0, 0);
		rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
		collider.isTrigger = true;
		offbody = false;
		detatchVictim();
		pulling = false;
	}
	
	void constrainDistanceToBody()
	{
		if (getMagnitude() > maxDistanceForSnap)
		{
			detatchVictim();
		}
        
		if (getMagnitude() > maxDistance)
		{
            float dist = Vector3.Distance(transform.localPosition, StartPosition());
			
            if (victimIsStatic())
            {
                grappelToFist(dist - maxDistance);
            }
            else
            {
				pullFist(dist - maxDistance);
            }
		}
	}
	void grabVictim(Collider2D col)
	{
		this.victimCollider = col;
		this.victim = col.gameObject;
		this.transform.position = victim.transform.position;
		collider.isTrigger = true;
		rigidbody2D.bodyType = RigidbodyType2D.Static;
		rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
		
		Physics2D.IgnoreCollision(victimCollider,bodyCollider,true);

		victimRigidBody().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		victimPreviousGravity = victimRigidBody().gravityScale;
		victimPreviousAngularDrag = victimRigidBody().angularDrag;
		victim.transform.rotation = Quaternion.identity;
		victimRigidBody().gravityScale = 100;
		victimRigidBody().angularDrag = 10;
	}
	void detatchVictim()
	{
		if (victim != null)
		{
			victimRigidBody().gravityScale = victimPreviousGravity;
			victimRigidBody().angularDrag = victimPreviousAngularDrag;
			victimRigidBody().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
			victimRigidBody().constraints = RigidbodyConstraints2D.None;
			Physics2D.IgnoreCollision(victimCollider, bodyCollider, false);
			victim = null;
		}
	}
	bool nearStart()
	{
		if (victim != null)
		{
			return victimCollider.bounds.Intersects(bodyCollider.bounds);
		}
		return Vector3.Distance(transform.localPosition,  StartPosition()) < tolerance;
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (offbody && victim == null)
		{
			if (((0x1 << col.gameObject.layer) & staticEnvLayerMask) != 0)
			{
				finishLaunchFist();
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
	
	void toggleFist()
	{
		collider.isTrigger = !collider.isTrigger;
	}
}
