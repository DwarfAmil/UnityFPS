using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickQuitBtn()
    {
        Application.Quit();
    }
}
