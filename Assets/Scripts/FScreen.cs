using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FScreen : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
