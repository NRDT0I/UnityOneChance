using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float rotationSpeed = 720f;
    public float jumpForce = 5f;
    public float gravity = -30f;
    public float airControl = 0.5f;
    public float deceleration = 5f;
    public float smoothAcceleration = 10f;

    private Vector3 moveDirection = Vector3.zero;
    public float verticalVelocity = 0f;
    private CharacterController controller;
    private Vector3 lastInputDirection = Vector3.zero;
    public Vector3 currentVelocity = Vector3.zero;

    private bool isAttachedToWinch = false; // Флаг подвешивания
    private CraneWinch currentWinch; // Текущая лебёдка

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isAttachedToWinch)
        {
            HandleWinchMovement();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    // 🔹 Обычное передвижение игрока
    void HandleNormalMovement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalMove, 0, verticalMove);
        inputDirection.Normalize();

        if (controller.isGrounded)
        {
            if (inputDirection.magnitude > 0.1f)
            {
                lastInputDirection = inputDirection;
                currentVelocity = Vector3.Lerp(currentVelocity, inputDirection * walkSpeed, smoothAcceleration * Time.deltaTime);
            }
            else
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
            }

            verticalVelocity = -0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            Vector3 airMove = inputDirection * walkSpeed * airControl;
            currentVelocity = Vector3.Lerp(currentVelocity, airMove, Time.deltaTime);
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = currentVelocity;
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

        if (inputDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastInputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (lastInputDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastInputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // 🔹 Движение на лебёдке
    void HandleWinchMovement()
    {
        if (currentWinch != null)
        {
            Vector3 moveDown = Vector3.down * currentWinch.descendSpeed * Time.deltaTime;
            controller.Move(moveDown);

            // Если игрок касается земли — отцепляем его
            if (controller.isGrounded)
            {
                DetachFromWinch();
            }
        }
    }

    // 🔹 Подключение к лебёдке
    public void AttachToWinch(CraneWinch winch)
    {
        isAttachedToWinch = true;
        currentWinch = winch;
        verticalVelocity = 0f;

        // Отключаем управление и фиксируем позицию
        controller.enabled = false;
        transform.position = winch.attachPoint.position;
        transform.parent = winch.attachPoint;

        // Включаем обратно, чтобы можно было спускаться
        controller.enabled = true;
    }

    // 🔹 Отключение от лебёдки
    public void DetachFromWinch()
    {
        isAttachedToWinch = false;
        controller.enabled = true;
        transform.parent = null;
        currentWinch = null;
        verticalVelocity = -0.5f; // Включаем гравитацию
    }
}
