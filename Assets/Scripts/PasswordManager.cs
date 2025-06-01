using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    public List<int> correctPassword = new List<int> { 7, 9, 1 };
    public GameObject doorToOpen;

    // ------------------------------------

    private List<int> enteredSequence = new List<int>();
    private List<TorchController> litTorches = new List<TorchController>();

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