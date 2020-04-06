using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour 
{
	public  bool isOn;

	public Color backgroundColor_ON; // #55B71A
    public Color backgroundColor_OFF; // #DE2E4D

    public Image backgroundImage;
	public RectTransform toggle;

	public GameObject handle;
	private RectTransform handleTransform;

	private float handleSize;
	private float posX_ON;
	private float posX_OFF;

    public float handleOffset;

	public GameObject icon_ON;
	public GameObject icon_OFF;


	public float speed;
	static float t = 0.0f;

	private bool switching = false;

    public GameObject toEnable;


	void Awake()
	{
		handleTransform = handle.GetComponent<RectTransform>();
		RectTransform handleRect = handle.GetComponent<RectTransform>();
		handleSize = handleRect.sizeDelta.x;
		float toggleSizeX = toggle.sizeDelta.x;
        posX_ON = (toggleSizeX / 2) - (handleSize/2) - handleOffset;
        posX_OFF = posX_ON * -1;

	}


	void Start()
	{
		if(isOn)
		{
            backgroundImage.color = backgroundColor_ON;
			handleTransform.localPosition = new Vector3(posX_ON, 0f, 0f);
            icon_ON.gameObject.SetActive(true);
            icon_OFF.gameObject.SetActive(false);
		}
		else
		{
            backgroundImage.color = backgroundColor_OFF;
			handleTransform.localPosition = new Vector3(posX_OFF, 0f, 0f);
            icon_ON.gameObject.SetActive(false);
            icon_OFF.gameObject.SetActive(true);
		}
	}
		
	void Update()
	{

		if(switching)
		{
			Toggle(isOn);
		}
	}

	public void DoYourStaff()
	{
        toEnable.SetActive(isOn);
	}

	public void Switching()
	{
		switching = true;
	}
		


	public void Toggle(bool toggleStatus)
	{
		if(!icon_ON.active || !icon_OFF.active)
		{
            icon_ON.SetActive(true);
            icon_OFF.SetActive(true);
		}
		
		if(toggleStatus)
		{
            backgroundImage.color = SmoothColor(backgroundColor_ON, backgroundColor_OFF);
			Transparency (icon_ON, 1f, 0f);
			Transparency (icon_OFF, 0f, 1f);
			handleTransform.localPosition = SmoothMove(handle, posX_ON, posX_OFF);
		}
		else 
		{
            backgroundImage.color = SmoothColor(backgroundColor_OFF, backgroundColor_ON);
			Transparency (icon_ON, 0f, 1f);
			Transparency (icon_OFF, 1f, 0f);
			handleTransform.localPosition = SmoothMove(handle, posX_OFF, posX_ON);
		}
			
	}


	Vector3 SmoothMove(GameObject toggleHandle, float startPosX, float endPosX)
	{
		
		Vector3 position = new Vector3 (Mathf.Lerp(startPosX, endPosX, t += speed * Time.deltaTime), 0f, 0f);
		StopSwitching();
		return position;
	}

	Color SmoothColor(Color startCol, Color endCol)
	{
		Color resultCol;
		resultCol = Color.Lerp(startCol, endCol, t += speed * Time.deltaTime);
		return resultCol;
	}

	CanvasGroup Transparency (GameObject alphaObj, float startAlpha, float endAlpha)
	{
		CanvasGroup alphaVal;
		alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
		alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, t += speed * Time.deltaTime);
		return alphaVal;
	}

	void StopSwitching()
	{
		if(t > 1.0f)
		{
			switching = false;

			t = 0.0f;
			switch(isOn)
			{
			case true:
				isOn = false;
				DoYourStaff();
				break;

			case false:
				isOn = true;
				DoYourStaff();
				break;
			}

		}
	}

}
