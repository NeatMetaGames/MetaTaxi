using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Client : MonoBehaviour,IPunObservable
{
    [SerializeField] PhotonView PV;
    public int PassengerAmount = 1;
    public bool AllPickedUp = false;
    public GameObject PickupIcon;
    public List<IndividualNpc> passengers = new List<IndividualNpc>();
    public SpawnPoint _SpawnPoint;
    public DropPoint _DropPoint;
    private int _PassengersPickedup = 0;
    public int PassengersPickedup
    {
        get { return _PassengersPickedup; }
        set
        {
            _PassengersPickedup = value;
            if (PassengersPickedup == PassengerAmount)
            {
                AllPickedUp = true;
                if(CommonReferences.Instance.myPlayer)
                {
                    this.transform.parent = CommonReferences.Instance.myPlayer.transform;
                    //CommonReferences.Instance.myCar.pickedUpClients.Add(this);
                    _SpawnPoint.occupied = false;


                    int reward = 0;
                    reward = (int)Vector2.Distance(_SpawnPoint.transform.position, _DropPoint.transform.position);
                    CommonReferences.Instance.myCar.reward = reward;
                    CommonReferences.Instance.myCar.passengerPickedUp = true;
                    _DropPoint.DropPointSprite.gameObject.SetActive(true);
                }
            }
        }
    }

    [Range(0,100)]public int chanceforOne;
    [Range(0,100)]public int chanceforTwo;
    [Range(0,100)]public int chanceforThree;
    [Range(0,100)]public int chanceforFour;

    public void EnableClient()
    {
        int randomizer = Random.Range(0, 100);

        if (randomizer < chanceforOne)
        {
            PassengerAmount = 1;
        }
        else if (randomizer < chanceforTwo)
        {
            PassengerAmount = 2;
        }
        else if (randomizer < chanceforThree)
        {
            PassengerAmount = 3;
        }
        else if (randomizer < chanceforFour)
        {
            PassengerAmount = 4;
        }
        else
            PassengerAmount = 5;


        PassengerAmount = Mathf.Clamp(PassengerAmount, 1, passengers.Count);
        for (int i = 0; i < PassengerAmount; i++)
        {
            int temp = i;
            passengers[temp].Initialize(temp);
        }

        CommonReferences.Instance.toScaleObjectsOn.Add(this.transform);

        EventTrigger eventTrigger = PickupIcon.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
    public void EnableClientForOthers()
    {
        for (int i = 0; i < PassengerAmount; i++)
        {
            int temp = i;
            passengers[temp].Initialize(temp);
        }

        EventTrigger eventTrigger = PickupIcon.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    private void OnPointerClickDelegate(PointerEventData data)
    {
        Debug.Log("trigger works");
        if (AllPickedUp || !_SpawnPoint.StayingNear) return;

        if (CommonReferences.Instance.myCar.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f) return;

        //if (CommonReferences.Instance.myCar.pickedUpClients.Count < CommonReferences.Instance.myCar.BagSize)
        if (!CommonReferences.Instance.myCar.passengerPickedUp)
        {
            Debug.Log("triggger definitely works");
            SendInCar(CommonReferences.Instance.myCar.transform);
            CommonReferences.Instance.myCar.UpdateFuelData();
            UIManager.Instance.pointer.Target = _DropPoint.transform;

            _DropPoint.hasClient = true;

            CommonReferences.Instance.myCar.dropPoint = _DropPoint;
            StartCoroutine(UIManager.Instance.tutorialCO("find destination"));
            CommonReferences.Instance.toScaleObjectsOn.Remove(this.transform);


            if (PV.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("Sent RPC");
                this.gameObject.SetActive(false);
                PV.RPC("DestroyThisNPC", RpcTarget.Others);
            }
        }
        else
        {
            Debug.Log("car full");
        }
    }
    [PunRPC]
    public void DestroyThisNPC()
    {
        if (PV != null)
        {
            _SpawnPoint.occupied = false;
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(PV);
            }
        }
    }

    [PunRPC]
    public void EnableThisClient(int spawn_id,int drop_id)
    {
       

        _SpawnPoint = CommonReferences.SpawnPoints[spawn_id];
        _DropPoint = CommonReferences.DropPoints[drop_id];

        _SpawnPoint.occupied = true;
        _SpawnPoint.myClient = this;
        
    }

    public void SetClientData()
    {
        if (!isInialized)
        {
            if (temp_dropID == -1 || temp_spawnID == -1) return;
            isInialized = true;

            _SpawnPoint = CommonReferences.SpawnPoints[temp_spawnID];
            _DropPoint = CommonReferences.DropPoints[temp_dropID];

            _SpawnPoint.occupied = true;
            _SpawnPoint.myClient = this;

            EnableClientForOthers();
        }
    }


    public int temp_spawnID=-1;
    public int temp_dropID=-1;

    public bool isInialized=false;
    public void SendInCar(Transform car)
    {
        for (int i = 0; i < PassengerAmount; i++)
        {
            int temp = i;
            passengers[temp].GetInCar(car);
            Debug.Log("passenger", passengers[temp]);
        }
        PickupIcon.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {                       
                stream.SendNext(temp_spawnID);
                stream.SendNext(temp_dropID);
            stream.SendNext(PassengerAmount);
        }
        else if (stream.IsReading)
        {
            if (!PV.IsMine)
            {
               
                                
                    temp_spawnID =(int) stream.ReceiveNext();
                    temp_dropID =(int) stream.ReceiveNext();
                PassengerAmount= (int)stream.ReceiveNext();
                if (!isInialized) { 
                  //  Debug.Log(" Testing : " + test1 + " " + test2);
                    SetClientData();
                }
            }
        }
    }
}
