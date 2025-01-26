using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float rotationSpeed = 720f;
    public float jumpForce = 5f;
    public float gravity = -30f; // Ускоренное падение для большей реалистичности
    public float airControl = 0.5f; // Управляемость в воздухе (0 - отсутствие управления)
    public float deceleration = 5f; // Замедление при остановке
    public float smoothAcceleration = 10f; // Ускорение для плавного набора скорости

    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;
    private CharacterController controller;
    private Vector3 lastInputDirection = Vector3.zero; // Запоминаем последнюю действительную направленность
    private Vector3 currentVelocity = Vector3.zero; // Текущая скорость движения

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Получение ввода
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        // Движение по плоскости
        Vector3 inputDirection = new Vector3(horizontalMove, 0, verticalMove);
        inputDirection.Normalize();

        // Управление при нахождении на земле
        if (controller.isGrounded)
        {
            if (inputDirection.magnitude > 0.1f)
            {
                // Запоминаем последнее действительное направление
                lastInputDirection = inputDirection;
                // Плавное увеличение скорости до максимальной
                currentVelocity = Vector3.Lerp(currentVelocity, inputDirection * walkSpeed, smoothAcceleration * Time.deltaTime);
            }
            else
            {
                // Замедление до остановки
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
            }

            verticalVelocity = -0.5f; // Удерживаем персонажа на земле

            // Прыжок
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // В воздухе управление ограничено
            Vector3 airMove = inputDirection * walkSpeed * airControl;
            currentVelocity = Vector3.Lerp(currentVelocity, airMove, Time.deltaTime);

            verticalVelocity += gravity * Time.deltaTime; // Применяем гравитацию
        }

        // Объединение движения по плоскости и вертикали
        Vector3 finalVelocity = currentVelocity;
        finalVelocity.y = verticalVelocity;

        // Перемещение персонажа
        controller.Move(finalVelocity * Time.deltaTime);

        // Поворот персонажа плавно в сторону движения (если есть движение по горизонтали)
        if (inputDirection.magnitude > 0.1f)
        {
            // Рассчитываем целевое направление
            Quaternion targetRotation = Quaternion.LookRotation(lastInputDirection);

            // Плавный поворот
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (lastInputDirection.magnitude > 0.1f)
        {
            // Если нет ввода, но направление было актуальным, продолжаем движение в прошлом направлении
            Quaternion targetRotation = Quaternion.LookRotation(lastInputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
