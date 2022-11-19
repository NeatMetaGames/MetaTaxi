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
    private bool StayingNear = false;
    private Transform Transporter;
    [SerializeField]private float delayTimer;
    private void Awake()
    {
        EventTrigger eventTrigger = this.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    private void OnPointerClickDelegate(PointerEventData data)
    {
        Debug.Log("trigger works");
        if (myClient == null || myClient.AllPickedUp || !StayingNear) return;

        if (CommonReferences.Instance.myCar.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f) return;

        if (CommonReferences.Instance.myCar.pickedUpClients.Count < CommonReferences.Instance.myCar.BagSize)
        {
            Debug.Log("triggger definitely works");
            myClient.SendInCar(Transporter);
            CommonReferences.Instance.myCar.UpdateFuelData();
            UIManager.Instance.pointer.Target = myClient._DropPoint.transform;
        }
        else
        {
            Debug.Log("car full");
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine && collision.CompareTag("car"))
        {
            StayingNear = true;
            Transporter = collision.transform;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine &&collision.CompareTag("car"))
        {
            StayingNear = false;
            Transporter = null;
        }
    }

  /*  private void Update()
    {
        if (myClient == null || myClient.AllPickedUp) return;
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
        }
    }*/

}
