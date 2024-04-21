using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class ApplyNow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Upload the Resume into the Account ng Business ng makikita yung resume ng mga Applicant 

    public void ChangeSuccess()
    {
        SceneManager.LoadScene("SUCCESS APPLY");
    }

    //CANCEL
     public void ChangeBusinessList()
    {
        SceneManager.LoadScene("TRY");
    }

}
