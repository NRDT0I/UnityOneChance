using UnityEngine;

public class CraneWinch : MonoBehaviour
{
    public Transform attachPoint;  // Точка крепления игрока
    public float descendSpeed = 3f; // Скорость спуска
    public float minHeight = 1f;    // Минимальная высота от земли
    private MovementScript playerMovement;
    private bool isPlayerAttached = false;

    void Update()
    {
        if (isPlayerAttached && playerMovement != null)
        {
            // Спускаем игрока вниз, пока он не достигнет minHeight
            if (playerMovement.transform.position.y > minHeight)
            {
                Vector3 moveDown = Vector3.down * descendSpeed * Time.deltaTime;
                playerMovement.GetComponent<CharacterController>().Move(moveDown);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            AttachPlayer(other.GetComponent<MovementScript>());
        }
    }

    private void AttachPlayer(MovementScript movement)
    {
        playerMovement = movement;
        isPlayerAttached = true;
        movement.AttachToWinch(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerAttached)
        {
            DetachPlayer();
        }
    }

    public void DetachPlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.DetachFromWinch();
            isPlayerAttached = false;
            playerMovement = null;
        }
    }
}
