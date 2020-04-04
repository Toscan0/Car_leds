using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteData : MonoBehaviour
{
    private bool enabled = false;


    public  void DeleteButtonClicked()
    {
        enabled = !enabled;
        this.gameObject.SetActive(enabled);
    }

    public void DisablePopUp()
    {
        enabled = false;
        this.gameObject.SetActive(enabled);
    }

    public void DeleteAllData()
    {
        // Delete all the data saved in the device
        PlayerPrefs.DeleteAll();

        enabled = false;
        this.gameObject.SetActive(enabled);
    }
}
