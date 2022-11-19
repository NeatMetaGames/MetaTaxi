using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    //public List<Client> myClients = new List<Client>();
    public bool hasClient;
    private bool StayingNear = false;
    [SerializeField] private float delayTimer;
    [SerializeField] public SpriteRenderer DropPointSprite;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine && (collision.CompareTag("Player") || collision.CompareTag("car")))
        {
            StayingNear = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine && (collision.CompareTag("Player") || collision.CompareTag("car")))
        {
            StayingNear = false;
        }
    }


    private void Update()
    {
        if (!hasClient) return;

        if (!UIManager.Instance.Step[10].SkipThisStep && !UIManager.Instance.Step[10].AdminSkip)
        {
            if (isOffScreen()) return;

            StartCoroutine(UIManager.Instance.tutorialCO("found destination"));
            UIManager.Instance.Step[10].ObjectToPoint = this.transform;
        }
        
        if (StayingNear)
        {
            if (UIManager.Instance.getStatedeliveredUI) { delayTimer = 0; return; }
            delayTimer += Time.deltaTime;
            if (delayTimer > 2)
            {
                if (CommonReferences.Instance.myCar.passengerPickedUp)
                {
                    delayTimer = 0;

                    /*CommonReferences.Instance.myCar.pickedUpClients.Remove(myClients[0]);
                    Destroy(myClients[0].gameObject);*/
                    //myClients.RemoveAt(0);
                    StartCoroutine(UIManager.Instance.tutorialCO("dropped people"));
                    Debug.Log("give Reward here" + CommonReferences.Instance.myCar.reward);

                    UIManager.Instance.ShowOrderDeliveredPanel(CommonReferences.Instance.myCar.reward);
                    CommonReferences.Instance.myCar.passengerPickedUp = false;
                    CommonReferences.Instance.myCar.dropPoint = null;
                    CommonReferences.Instance.myCar.reward = 0;
                    hasClient = false;
                    UIManager.Instance.pointer.Target = null;
                    CommonReferences.Instance.myCar.UpdateFuelData();
                }
            }
        }
        else
        {
            delayTimer = 0;
        }
    }

    private bool isOffScreen()
    {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        bool isOffScreen = targetPositionScreenPoint.x <= 300 || targetPositionScreenPoint.x >= Screen.width - 300 || targetPositionScreenPoint.y <= 300 || targetPositionScreenPoint.y >= Screen.height - 300;
        return isOffScreen;
    }
}
