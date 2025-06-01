using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    public List<int> correctPassword = new List<int> { 7, 9, 1 };
    public GameObject doorToOpen;

    // --- NUEVAS VARIABLES PARA EL AUDIO ---
    public AudioClip successSound;
    private AudioSource audioSource;
    // ------------------------------------

    private List<int> enteredSequence = new List<int>();
    private List<TorchController> litTorches = new List<TorchController>();

    void Awake() // Cambiado de Start a Awake para asegurar que audioSource se obtiene antes
    {
        // Intentar obtener el AudioSource del mismo GameObject
        audioSource = GetComponent<AudioSource>();

        // Opcional: si el AudioSource no está en este GameObject, podrías buscarlo
        // o requerir que se asigne manualmente en el Inspector.
        // Por ahora, asumimos que está en el mismo GameObject.
        if (audioSource == null)
        {
            Debug.LogWarning("PasswordManager no encontró un AudioSource en este GameObject. Se añadirá uno.");
            // Añadir un AudioSource si no existe (opcional, pero puede ser útil)
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Asegurar que no se reproduzca al inicio
        }

        // Validar que el successSound esté asignado
        if (successSound == null)
        {
            Debug.LogWarning("PasswordManager: No se ha asignado 'successSound'. No se reproducirá sonido de éxito.");
        }
    }

    void Start()
    {
        if (doorToOpen != null)
        {
            // doorToOpen.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PasswordManager: No se ha asignado 'doorToOpen'. La puerta no se abrirá.");
        }
    }

    public void TorchActivated(int number, TorchController torchController)
    {
        if (litTorches.Contains(torchController))
        {
            return;
        }
        enteredSequence.Add(number);
        litTorches.Add(torchController);
        Debug.Log("Antorcha " + number + " activada. Secuencia actual: " + string.Join(", ", enteredSequence));
        if (enteredSequence.Count == correctPassword.Count)
        {
            CheckPassword();
        }
        else if (enteredSequence.Count > correctPassword.Count)
        {
            ResetPuzzle();
        }
    }

    void CheckPassword()
    {
        bool isCorrect = enteredSequence.SequenceEqual(correctPassword);

        if (isCorrect)
        {
            Debug.Log("¡Contraseña CORRECTA!");
            OpenDoor();

            // --- REPRODUCIR SONIDO DE ÉXITO ---
            if (audioSource != null && successSound != null)
            {
                audioSource.PlayOneShot(successSound); // PlayOneShot es bueno para efectos de sonido que no se interrumpen
                // Alternativamente, si quieres más control (y el AudioClip ya está asignado al AudioSource):
                // audioSource.clip = successSound; // Si no estaba asignado al AudioSource
                // audioSource.Play();
            }
            // ------------------------------------
        }
        else
        {
            Debug.Log("Contraseña INCORRECTA. Reseteando antorchas.");
            // Opcional: Podrías tener un sonido de "fallo" aquí también
            // if (audioSource != null && failureSound != null) {
            //     audioSource.PlayOneShot(failureSound);
            // }
            ResetPuzzle();
        }
    }

    void OpenDoor()
    {
        if (doorToOpen != null)
        {
            Debug.Log("Abriendo puerta...");
            doorToOpen.SetActive(false);
        }
        else
        {
            Debug.Log("PUERTA ABIERTA (simulado - no hay objeto puerta asignado)");
        }
    }

    public void ResetPuzzle()
    {
        Debug.Log("Reseteando puzzle...");
        foreach (TorchController torch in litTorches)
        {
            if (torch != null)
            {
                torch.SetState(false);
            }
        }
        enteredSequence.Clear();
        litTorches.Clear();
    }
}