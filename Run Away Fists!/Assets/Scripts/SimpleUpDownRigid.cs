///BASED FROM UNITY PLATFORMER 2D PACK

using UnityEngine;

namespace PC2D
{
	public class SimpleUpDownRigid : MonoBehaviour
	{
		public float upDownAmount;
        float currentOffset = 0;
		public float speed = 0.25f;
		public Rigidbody2D _mpMotor;
        int direction = 1;
		private float _startingY;

		// Use this for initialization
		void Start()
		{
			_startingY = transform.position.y;
		}

		// Update is called once per frame
		void FixedUpdate()
		{
            if (currentOffset > upDownAmount)
            {
				direction = -1;
            }
            if (-currentOffset > upDownAmount)
            {
				direction = 1;
            }
			
            currentOffset += speed * direction;
			
            _mpMotor.MovePosition(
                new Vector2(
                    _mpMotor.transform.position.x,
                    _startingY + currentOffset
                )
            );

		}
	}
}
