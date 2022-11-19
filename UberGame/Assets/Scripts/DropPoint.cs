using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    public List<Client> myClient = new List<Client>();
    private bool StayingNear = false;
    [SerializeField] private float delayTimer;

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
        if (myClient.Count == 0) return;
        if (StayingNear)
        {
            if (UIManager.Instance.getStatedeliveredUI) { delayTimer = 0; return; }
            delayTimer += Time.deltaTime;
            if (delayTimer > 2)
            {
                if (myClient[0].AllPickedUp)
                {
                    delayTimer = 0;
                    int reward = 0;
                    reward = (int)Vector2.Distance(myClient[0]._SpawnPoint.transform.position, myClient[0]._DropPoint.transform.position);
                    Debug.Log("get yer money ya filthy animal : " + reward);
                    CommonReferences.Instance.myCar.pickedUpClients.Remove(myClient[0]);
                    Destroy(myClient[0].gameObject);
                    myClient.RemoveAt(0);
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
}
