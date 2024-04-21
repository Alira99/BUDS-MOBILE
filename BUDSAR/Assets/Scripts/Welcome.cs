using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Welcome : MonoBehaviour
{
   public void ChangeRegister()
    {
        SceneManager.LoadScene("REGISTER");
    }

    public void ChangeLogin()
    {
        SceneManager.LoadScene("LOGIN");
    }
}
