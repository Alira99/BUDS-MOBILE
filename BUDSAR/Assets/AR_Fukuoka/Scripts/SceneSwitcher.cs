// SceneSwitcher.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToBusinessProfileScene()
    {
        SceneManager.LoadScene("BusinessProfileScene");
    }
}
