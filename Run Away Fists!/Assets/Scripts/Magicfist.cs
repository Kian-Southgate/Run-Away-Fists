
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magicfist : MonoBehaviour
{
	Vector3 offset = new Vector3(1.5f, 0.0f, 0.0f);
	public int maxDistance = 6;
	public float launchSpeed = 0.5F;
	public float pullSpeed = 0.5f;
	public float grappelSpeed = 0.25f;
	public LayerMask staticEnvLayerMask;
	public Rigidbody2D rigidbody2D;
	public BoxCollider2D collider;
	public GameObject body;
	public PlatformerMotor2D platformerMotor2D;
	public float previousGravity = 0;
	GameObject victim;
	Vector3 targetPos;
	public float tolerance = 0.25f;
	public bool offbody = false;

	Vector3 StartPosition()
	{
		return new Vector3(body.transform.localPosition.x + offset.x, body.transform.localPosition.y + offset.y, body.transform.localPosition.z + offset.z);
	}
	void Start()
	{
		offset = new Vector3(transform.localPosition.x - body.transform.localPosition.x, transform.localPosition.y - body.transform.localPosition.y, transform.localPosition.z - body.transform.localPosition.z);
		
		previousGravity = platformerMotor2D.gravityMultiplier;
	}

	void FixedUpdate()
	{
		if (Input.GetMouseButton(0))
		{
			if (victim == null && rigidbody2D.bodyType != RigidbodyType2D.Dynamic) 
			{
				if (offbody == false)
				{
					targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					targetPos.z = transform.position.z;
					transform.position = Vector3.MoveTowards(transform.position, targetPos, launchSpeed);
					offbody = true;
				} else {
					transform.position = Vector3.MoveTowards(transform.position, targetPos, launchSpeed);
				}
			}
		} else if (atStart() == false)
		{
			finishLaunchFist();
		}
		
		if (getMagnitude() > maxDistance)
		{
			finishLaunchFist();
		}

		if ((Input.GetMouseButton(1)) && atStart() == false)
		{
			pullFist();
		}

		if (Input.GetKey(KeyCode.LeftShift) && victim != null)
		{
			platformerMotor2D.gravityMultiplier = 0;
			grappelToFist(grappelSpeed * 2);
		}
        else 
		{
			platformerMotor2D.gravityMultiplier = previousGravity;
		}

		if (nearStart() && Input.GetMouseButton(0) == false)
		{
			resetFist();
		}

		constrainDistanceToBody();

		if (victim != null)
		{
			victim.transform.position = this.transform.position;
		}

		if (offbody == false)
		{
			transform.localPosition = StartPosition();
		}
	}

	bool atStart()
	{
		return Vector3.Distance(transform.localPosition, StartPosition()) < tolerance;
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
	void pullFist()
	{
		detatchStaticVictim();
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, StartPosition(), pullSpeed);
	}

    void detatchStaticVictim()
    {
        if (victimIsStatic())
        {
            victim = null;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
	}
    bool victimIsStatic()
    {
        if (victim != null)
        {
            Rigidbody2D otherBody = victim.gameObject.GetComponent<Rigidbody2D>();
            if (otherBody.bodyType == RigidbodyType2D.Static)
            {
                return true;
            }
        }
        return false;
    }
    void grappelToFist(float speed)
	{
		body.transform.localPosition = Vector3.MoveTowards(body.transform.localPosition, transform.localPosition, speed);
	}
	public  void resetFist()
	{
		rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
		rigidbody2D.velocity = new Vector2(0, 0);
		rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
		collider.isTrigger = true;
		offbody = false;
		victim = null;
	}
	void constrainDistanceToBody()
	{
        
		if (getMagnitude() > maxDistance)
		{
            float dist = Vector3.Distance(transform.localPosition, StartPosition());

            if (victimIsStatic())
            {
                grappelToFist(dist - maxDistance);
            }
            else
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, StartPosition(), dist - maxDistance);
            }
		}
	}


	

	void grabVictim(GameObject victim)
	{
		this.victim = victim;
		this.transform.position = victim.transform.position;
		rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	

	bool nearStart()
	{
		return Vector3.Distance(transform.localPosition,  StartPosition()) < 0.5f;
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
				grabVictim(col.gameObject);
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
		Debug.Log(other.layer);
		

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
