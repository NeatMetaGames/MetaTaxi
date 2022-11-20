using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnPoint : MonoBehaviour
{
    public bool occupied;
    public Client myClient;
    public Collider2D myCollider;
    public bool StayingNear = false;
    private Transform Transporter;
    [SerializeField]private float delayTimer;
  
    public void OnPointerClickDelegate(PointerEventData data)
    {
        #region temp
        /* Debug.Log("trigger works");
         if (myClient == null || myClient.AllPickedUp || !StayingNear) return;

         if (CommonReferences.Instance.myCar.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f) return;

         //if (CommonReferences.Instance.myCar.pickedUpClients.Count < CommonReferences.Instance.myCar.BagSize)
         if(!CommonReferences.Instance.myCar.passengerPickedUp)
         {
             Debug.Log("triggger definitely works");
             myClient.SendInCar(Transporter);
             CommonReferences.Instance.myCar.UpdateFuelData();
             UIManager.Instance.pointer.Target = myClient._DropPoint.transform;
             if (myClient)
             {
                 myClient._DropPoint.hasClient = true;

                 CommonReferences.Instance.myCar.dropPoint = myClient._DropPoint;

             }

             if (myClient.GetComponent<PhotonView>().IsMine || PhotonNetwork.IsMasterClient)
             {
                 PhotonNetwork.Destroy(myClient.gameObject);
             }

         }
         else
         {
             Debug.Log("car full");
         }*/
        #endregion
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine && collision.CompareTag("car"))
        {
            StayingNear = true;
            Transporter = collision.transform;
            if (myClient)
            {
                myClient.PickupIcon.SetActive(!myClient.AllPickedUp);
                var Step = UIManager.Instance.Step;
                int ID = Step.FindIndex(x => x.Code == "pick up");
                if (!Step[ID].SkipThisStep && !Step[ID].AdminSkip)
                {
                    StartCoroutine(UIManager.Instance.tutorialCO("pick up"));
                    Step[ID].ObjectToPoint = myClient.PickupIcon.transform;
                }

            }
        }

        if(collision.GetComponentInParent<PhotonView>().IsMine && collision.CompareTag("Player"))
        {
            UIManager.Instance.ShowInformationMsg("Come With A Car" , 3);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine &&collision.CompareTag("car"))
        {
            StayingNear = false;
            Transporter = null;
            if(myClient)
                myClient.PickupIcon.SetActive(false);
        }
    }


    private bool isOffScreen()
    {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        bool isOffScreen = targetPositionScreenPoint.x <= 300 || targetPositionScreenPoint.x >= Screen.width - 300 || targetPositionScreenPoint.y <= 300 || targetPositionScreenPoint.y >= Screen.height - 300;
        return isOffScreen;
    }
    private void Update()
    {
        if (myClient == null) return;

        if (!UIManager.Instance.Step[7].SkipThisStep && !UIManager.Instance.Step[7].AdminSkip)
        {
            if (isOffScreen()) return;
            StartCoroutine(UIManager.Instance.tutorialCO("found people"));
           
                UIManager.Instance.Step[7].ObjectToPoint = myClient.transform;
        }
        
       /* if (myClient == null || myClient.AllPickedUp) return;
        if (StayingNear)
        {
            if (UIManager.Instance.getStatedeliveredUI) { delayTimer = 0; return; }
            delayTimer += Time.deltaTime;
            if (delayTimer > 2)
            {
                if (!myClient.AllPickedUp)
                {
                    delayTimer = 0;
                    Debug.Log("picked up client");
                    if (Transporter != null)
                    {
                        myClient.SendInCar(Transporter);
                    }
                    CommonReferences.Instance.myCar.UpdateFuelData();
                }
            }
        }
        else
        {
            delayTimer = 0;
        }*/
    }

}
