using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Hud hud;

    private static int vidas = 3;

    public static class DatosJuego
    {
        public static string idProximoSpawn = "";
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Eliminar duplicados
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hud = FindFirstObjectByType<Hud>(); // Vuelve a encontrar el HUD en la nueva escena
        if (hud != null)
        {
            // Actualiza el HUD según la cantidad actual de vidas
            for (int i = vidas; i < 3; i++)
            {
                hud.DesactivarVida(i);
            }
        }
    }

    public void PerderVidas()
    {
        vidas--;
        hud.DesactivarVida(vidas);
        if (vidas <= 0)
        {
            SceneManager.LoadScene(4);
            vidas = 3;
        }
        else
        {
            ResetLevel();
        }
        
    }

    public void RecuperarVidas()
    {
        vidas++;

    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
