using UnityEngine;

public class MovementScript : MonoBehaviour
{
	public float speed;
	public float rotationSpeed;
	public float jumpSpeed;
	private float ySpeed;
	private CharacterController conn;
	public bool isGrounded;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        conn = GetComponent<CharacterController>();
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
       //transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
       conn.SimpleMove(moveDirection * magnitude * speed);
       
       ySpeed += Physics.gravity.y * Time.deltaTime;
       if(Input.GetButtonDown("Jump"))
       {
       	ySpeed = -0.5f;
       	isGrounded = false;
       }
       
       Vector3 vel = moveDirection * magnitude;
       vel.y = ySpeed;
       //transform.Translate(vel * Time.deltaTime);
       conn.Move(vel * Time.deltaTime);
       
       if(conn.isGrounded)
       {
       	ySpeed = -0.5f;
       	isGrounded = true;
       	if (Input.GetButtonDown("Jump"))
       	{
       		ySpeed = jumpSpeed;
       		isGrounded = false;
       	}
       }
       
       if(moveDirection != Vector3.zero)
       {
       	Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
       	transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
       }
    }
}
