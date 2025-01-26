using UnityEngine;

public class MovementScript : MonoBehaviour
{
	public float speed;
	public float rotationSpeed;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       float horizontalMove = Input.GetAxis("Horizontal");
       float verticalMove = Input.GetAxis("Vertical");
       
       Vector3 moveDirection = new Vector3(horizontalMove, 0, verticalMove);
       moveDirection.Normalize();
       float magnitude = moveDirection.magnitude;
       magnitude = Mathf.Clamp01(magnitude);
       transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
       
       if(moveDirection != Vector3.up)
       {
       	Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
       	transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
       }
    }
}
