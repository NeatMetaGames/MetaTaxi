using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour
{
    public Image img;
    public GameObject rotator;
    public Sprite[] sprites;
    private Transform target;

    [SerializeField] float minx;
    [SerializeField] float miny;
    public Transform Target { get { return target; } 
        set
        {
            target = value;
            if (target != null)
            {
                if (target.name.Equals("gas"))
                {
                    //GAS ICON
                    ChangeIcon(0);
                }

                else if (target == CommonReferences.Instance.myCar.transform)
                {
                    //CAR ICON
                    ChangeIcon(1);
                }

                else
                {
                    ChangeIcon(2);
                }
            }
        }
    }
    private Camera mainCam;
    public float zRotateOffset;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        showDirection();
    }

    public void ChangeIcon(int spriteIndex)
    {
        img.color = new Color(1, 1, 1, 1);
        img.sprite = sprites[spriteIndex];
    }
    public void showDirection()
    {
        if (Target == null)
        {
            if (img.gameObject.activeSelf)
                img.gameObject.SetActive(false);
            return;
        }
        img.gameObject.SetActive(isOffScreen());
        
        float maxx = Screen.width - minx;

     
        float maxy = Screen.height - miny;


        Vector2 pos = mainCam.WorldToScreenPoint(Target.position);

        pos.x = Mathf.Clamp(pos.x, minx + 100 , maxx - 100);
        pos.y = Mathf.Clamp(pos.y, miny + 100, maxy - 100);

        img.transform.position = pos;      

        if(!isOffScreen())
        {
            rotator.SetActive(false);
            if (UIManager.Instance.PlayingTutorial && !UIManager.Instance.tutStepInProgress)
            {
                if (Target.CompareTag("Restaurant") )
                {
                    int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "found restaurant");
                    if (!UIManager.Instance.Step[ID].SkipThisStep)
                    {
                        UIManager.Instance.Step[ID].ObjectToPoint = Target;
                        StartCoroutine(UIManager.Instance.tutorialCO("found restaurant"));
                    }
                }
                if (Target.CompareTag("House"))
                {
                    int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "found house");
                    if (!UIManager.Instance.Step[ID].SkipThisStep)
                    {
                        UIManager.Instance.Step[ID].ObjectToPoint = Target;
                        StartCoroutine(UIManager.Instance.tutorialCO("found house"));
                    }
                    
                }
            }
        }
        else
        {
            rotator.SetActive(true);
            RotatePointerTowardsTarget();
        }
    }

    private void RotatePointerTowardsTarget()
    {
        if (Target != null)
        {
            Vector3 toPos = Target.position;
            Vector3 from = mainCam.transform.position;
            from.z = 0;
            Debug.DrawLine(from, toPos, Color.red);
            Vector3 dir = (toPos - from).normalized;
            rotator.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, zRotateOffset+ Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg);
        }
    }

    [SerializeField] float borderSize = 150f;

    private bool isOffScreen()
    {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(Target.transform.position);      
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
        return isOffScreen;
    }
}
