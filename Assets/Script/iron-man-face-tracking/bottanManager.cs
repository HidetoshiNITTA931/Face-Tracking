using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bottanManager : MonoBehaviour
{
    private HelmetOpener helmetOpener1;
    private HelmetOpener helmetOpener2;

    public GameObject ironmanChild1;
    public GameObject ironmanChild2;

    private void Start()
    {
        
    }

    // Start is called before the first frame update
    public void OpenClose()
    {
        helmetOpener1 = ironmanChild1.GetComponent<HelmetOpener>();
        helmetOpener2 = ironmanChild2.GetComponent<HelmetOpener>();

        if (helmetOpener1.GetStatus() == 0)
        {
            helmetOpener1.Open();
        }
        else
        {
            helmetOpener1.Close();
        }

        if (helmetOpener2.GetStatus() == 0)
        {
            helmetOpener2.Open();
        }
        else
        {
            helmetOpener2.Close();
        }
    }
    
}
