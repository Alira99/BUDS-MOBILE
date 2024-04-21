using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class demo : MonoBehaviour
{
    public void ChangeProfile()
    {
        SceneManager.LoadScene("PROFILE");
    }

    public void ChangeResume()
    {
        SceneManager.LoadScene("VIEW RESUME");
    }

    public void ChangeBusinessList()
    {
        SceneManager.LoadScene("TRY");
    }

    public void ChangeHome()
    {
        SceneManager.LoadScene("HOMESCREEN");
    }

    public void ChangeApplyNow()
    {
        SceneManager.LoadScene("APPLYNOW");
    }

     public void ChangeJobHiring()
    {
        SceneManager.LoadScene("JOB HIRING TRY");
    }

    public void BackToProfile()
    {
        SceneManager.LoadScene("PROFILE");
    }

}