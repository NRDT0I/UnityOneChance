using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera vCam2D; // Виртуальная камера для 2D
    public CinemachineVirtualCamera vCam3D; // Виртуальная камера для 3D

    [Header("Player")]
    public Transform player; // Ссылка на игрока

    [Header("Camera Positions")]
    private Vector3 initialCameraPosition2D; // Начальная позиция камеры в 2D

    private CinemachineComposer vCam3DComposer; // Для доступа к компоненту Composer в 3D камере

    private void Start()
    {
        // Убедимся, что при старте активна 2D камера
        vCam2D.Priority = 10;
        vCam3D.Priority = 0;

        // Сохраняем начальную позицию 2D камеры
        if (vCam2D.Follow != null)
        {
            initialCameraPosition2D = vCam2D.Follow.position;
        }

        // Инициализация компонента для 3D камеры
        vCam3DComposer = vCam3D.GetCinemachineComponent<CinemachineComposer>();

        // Блокируем 2D камеру от изменения позиции
        vCam2D.transform.position = initialCameraPosition2D;

        // Блокируем вращение 2D камеры
        vCam2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Вход в 3D зону. Переключение на 3D камеру.");
            SwitchTo3DCamera();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Выход из 3D зоны. Возврат к 2D камере.");
            SwitchTo2DCamera();
        }
    }

    private void SwitchTo3DCamera()
    {
        // Устанавливаем приоритет 3D камеры выше
        vCam2D.Priority = 0;
        vCam3D.Priority = 10;

        // Убедимся, что 3D камера следует за игроком
        vCam3D.Follow = player;

        // Сбрасываем смещение для предотвращения неожиданного поведения
        if (vCam3DComposer != null)
        {
            vCam3DComposer.m_TrackedObjectOffset = Vector3.zero;
        }
    }

    private void SwitchTo2DCamera()
    {
        // Устанавливаем приоритет 2D камеры выше
        vCam2D.Priority = 10;
        vCam3D.Priority = 0;

        // Проверяем, что 3D камера не влияет на 2D камеру
        if (vCam3DComposer != null)
        {
            vCam3DComposer.m_TrackedObjectOffset = Vector3.zero; // Сброс смещения в 3D камере
        }

        // Возвращаем 2D камеру в начальную позицию
        vCam2D.transform.position = initialCameraPosition2D;

        // Блокируем вращение 2D камеры
        vCam2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // Убедимся, что камера продолжает следить за игроком
        vCam2D.Follow = player;

        // Сбрасываем смещение
        CinemachineComposer composer = vCam2D.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_TrackedObjectOffset = Vector3.zero;
        }
    }
}
