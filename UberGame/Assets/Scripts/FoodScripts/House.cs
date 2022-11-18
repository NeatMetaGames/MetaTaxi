using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class House : MonoBehaviour
{
    private GameObject icon;
    private Collider2D iconCollider;
    public List<SpriteRenderer> ClientPic = new List<SpriteRenderer>();

    private int HomeID = -1;
    private void Awake()
    {
        icon = transform.GetChild(0).gameObject;
        iconCollider = icon.GetComponent<Collider2D>();
        HomeID = transform.GetSiblingIndex();

       /* EventTrigger eventTrigger = this.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);*/
    }

   /* private void OnPointerClickDelegate(PointerEventData data)
    {

    }*/

    #region Trigger Interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine)
        {
            StayingNear = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine)
        {
            StayingNear = false;
        }
    }
    #endregion
    bool StayingNear = false;
    float DeliveryTimer;
    private void Update()
    {
        if(StayingNear)
        {
            if (UIManager.Instance.getStatedeliveredUI) { DeliveryTimer = 0; return; }
            DeliveryTimer += Time.deltaTime;

            if (DeliveryTimer > 2)
            {
                if (true)
                {
                    DeliveryTimer = 0;
                    CommonReferences.Instance.myCar.UpdateFuelData();
                }
            }
        }
        else
        {
            DeliveryTimer = 0;
        }
    }
    public void SetClientPic(List<Sprite> ClientFeatures)
    {
        for (int i = 0; i < ClientFeatures.Count; i++)
        {
            int temp = i;
            ClientPic[temp].sprite = ClientFeatures[temp];
        }
    }
}
