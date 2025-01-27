using UnityEngine;
using Cinemachine;

public class CameraSwitchTrigger : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera vCam2D; // Ссылка на 2D камеру
    public CinemachineVirtualCamera vCam3D; // Ссылка на 3D камеру

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, если объект — это игрок
        if (other.CompareTag("Player"))
        {
            SwitchTo3DView();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Возврат к 2D камере при выходе игрока из триггера
        if (other.CompareTag("Player"))
        {
            SwitchTo2DView();
        }
    }

    private void SwitchTo2DView()
    {
        Debug.Log("Переключение на 2D камеру");
        vCam2D.Priority = 10; // Устанавливаем приоритет 2D камеры выше
        vCam3D.Priority = 0;  // Устанавливаем приоритет 3D камеры ниже
    }

    private void SwitchTo3DView()
    {
        Debug.Log("Переключение на 3D камеру");
        vCam2D.Priority = 0;  // Устанавливаем приоритет 2D камеры ниже
        vCam3D.Priority = 10; // Устанавливаем приоритет 3D камеры выше
    }
}
