using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalParent : MonoBehaviour
{
	Vector3 offset = new Vector3(1.5f, 0.0f, 0.0f);
	public GameObject body;
	public float tolerance = 0.25f;
	// Start is called before the first frame update
	Magicfist magicFist;
	void Start()
	{
		offset = new Vector3(transform.localPosition.x - body.transform.localPosition.x, transform.localPosition.y - body.transform.localPosition.y, transform.localPosition.z - body.transform.localPosition.z);
	}

    // Update is called once per frame
    void Update()
    {
		if (nearStart() && Input.GetKey("q") == false)
		{
			magicFist.resetFist();
		}
    }

	Vector3 StartPosition()
	{
		return new Vector3(body.transform.localPosition.x + offset.x, body.transform.localPosition.y + offset.y, body.transform.localPosition.z + offset.z);
	}

	bool nearStart()
	{
		return Vector3.Distance(transform.localPosition, StartPosition()) < 0.5f;
	}
}
