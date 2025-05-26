using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_inicio : MonoBehaviour
{
    public int numeroEscena;

    public void Iniciar()
    {
        if (Application.CanStreamedLevelBeLoaded(numeroEscena))
        {
            SceneManager.LoadScene(numeroEscena);
        }
        else
        {
            Debug.LogError("No se puede cargar la escena " + numeroEscena + " porque no está en el build settings.");
        }
    } 

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }   
}
