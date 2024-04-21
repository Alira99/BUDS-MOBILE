using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SubmitResume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeApplyNow()
    {
        SceneManager.LoadScene("APPLYNOW");
    }

    // Upload the Resume into the Account ng Business ng makikita yung resume ng mga Applicant 
}
