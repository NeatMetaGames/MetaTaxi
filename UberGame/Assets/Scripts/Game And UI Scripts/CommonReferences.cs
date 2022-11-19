using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Cinemachine;
using Random = UnityEngine.Random;

public class CommonReferences : MonoBehaviour
{

    
    public static CommonReferences Instance;
    public Transform SpawnPointsParent;
    public Transform DropPointsParent;
    public Transform GasStationparent;

    public static List<DropPoint> DropPoints = new List<DropPoint>();
    public static List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();
    public static List<Transform> GasStations = new List<Transform>();

    public Transform OrderUIParent;

    public PhotonView myPV;
    public PlayerController myPlayer;
    public CarController myCar;
    public GameObject carLight;

    public static Action OnOrderDispatched;
    public static Action<int> OnDisplayHouse;

    public CinemachineVirtualCamera camera_player;
    public CinemachineVirtualCamera camera_car;
    public CinemachineVirtualCamera camera_map;

    [SerializeField]public List<Transform> toScaleObjectsOn = new List<Transform>();

    public Transform[] playerPoz;

    [SerializeField] CAMERA_TYPE lastCamera;
    public void SwitchCamera(CAMERA_TYPE whichCamera)
    {
        switch (whichCamera)
        {
            case CAMERA_TYPE.CAR:
                {
                    lastCamera = whichCamera;
                    camera_car.Priority = 10;
                    camera_player.Priority = 0;
                    camera_map.Priority = 0;
                    break;
                }
               
            case CAMERA_TYPE.PLAYER:
                {
                    lastCamera = whichCamera;
                    camera_car.Priority = 0;
                    camera_player.Priority = 10;
                    camera_map.Priority = 0;
                    break;
                }
            case CAMERA_TYPE.MAP:
                {
                    camera_car.Priority = 0;
                    camera_player.Priority = 0;
                    camera_map.Priority = 10;                  
                     
                    break;
                }
        }
    }

    bool isMapOpen = false;
    public void ToggleMap()
    {
        if (!isMapOpen)
        {
            if (myPlayer.DisableInputs) myPlayer.DisableInputs = false;
            if (myCar.DisableInputs) myCar.DisableInputs = false;


            isMapOpen = true;
            StartCoroutine(UIManager.Instance.tutorialCO("map in"));
            if (myPlayer._pState == PlayerState.WORLD)
            {
                myPlayer.canMove = false;
            }
            else
            {
                myCar.canDrive=false;
            }

            MouseMover.drag = true;
            SwitchCamera(CAMERA_TYPE.MAP);
            for (int i = 0; i < toScaleObjectsOn.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn[i].gameObject);
                LeanTween.scale(toScaleObjectsOn[i].gameObject, Vector3.one *5, 0.8f).setOnComplete(() =>
                {
                   /* LeanTween.moveLocalY(toScaleObjectsOn[i].gameObject, 2, 0.5f).setFrom(0).setLoopPingPong();
                    LeanTween.scaleY(toScaleObjectsOn[i].gameObject, 2 * 1.25f, 0.5f).setFrom(2).setLoopPingPong();*/
                });
            }
        }
        else
        {

            isMapOpen = false;

            if (myPlayer._pState == PlayerState.WORLD)
            {
                myPlayer.canMove = true;
            }
            else
            {
                myCar.canDrive = true;
            }


            MouseMover.drag = false;
            myPlayer.canMove = true;
            SwitchCamera(lastCamera);
            //StartCoroutine(UIManager.Instance.tutorialCO("close pending orders"));

            for (int i = 0; i < toScaleObjectsOn.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn[i].gameObject);
                LeanTween.scale(toScaleObjectsOn[i].gameObject, Vector3.one,0.8f).setOnComplete(() =>
                {
                   /* LeanTween.moveLocalY(toScaleObjectsOn[i].gameObject, 2, 0.5f).setFrom(0).setLoopPingPong();
                    LeanTween.scaleY(toScaleObjectsOn[i].gameObject, 1.25f, 0.5f).setFrom(1).setLoopPingPong();*/
                });
            }
        }
    }

    internal void SetupCameras()
    {
        camera_car.Follow = myCar.transform;
        camera_car.LookAt = myCar.transform;


        camera_player.Follow = myPlayer.transform;
        camera_player.LookAt = myPlayer.transform;

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        PhotonNetwork.SendRate = 10;

        #region Populate SpawnPoints
        int SpawnCount = SpawnPointsParent.childCount;
        for (int i = 0; i < SpawnCount; i++)
        {
            int temp = i;
            var s = SpawnPointsParent.GetChild(temp).GetComponent<SpawnPoint>();
            SpawnPoints.Add(s);
        }
        #endregion
        #region populate DropPoints
        int HouseCount = DropPointsParent.childCount;
        for (int i = 0; i < HouseCount; i++)
        {
            int temp = i;
            var d = DropPointsParent.GetChild(temp).GetComponent<DropPoint>(); 
            DropPoints.Add(d);
        }
        #endregion
        #region populate GasStations
        int GasStationCount = GasStationparent.childCount;
        if (GasStationCount > 0)
        {
            for (int i = 0; i < GasStationCount; i++)
            {
                int temp = i;
                var G = GasStationparent.GetChild(temp);
                GasStations.Add(G);
            }
        }
        #endregion
    }

   /* bool firstClient = true;*/
    public void SpawnClient()
    {
        List<SpawnPoint> freeSpawnPoints = new List<SpawnPoint>();
        foreach (var item in SpawnPoints)
        {
            if (!item.occupied)
            {
               /* if (firstClient)*/
                {
                    if (Vector2.Distance(item.transform.position, myCar.transform.position) < 20)
                    {
                        freeSpawnPoints.Add(item);
                       /* firstClient = false;*/
                    }
                }
               /* else
                    freeSpawnPoints.Add(item);*/
            }
        }
        if (freeSpawnPoints.Count == 0)
        {
            Debug.Log("no spawn point found");
            Invoke(nameof(SpawnClient), 2.5f);
            return;
        }


        var randomSpawnPointID = Random.Range(0, freeSpawnPoints.Count);
        var spawnPoint = freeSpawnPoints[randomSpawnPointID];
        int orignalSpawnId = SpawnPoints.IndexOf(spawnPoint);

        var randomDropPointID = Random.Range(0, DropPoints.Count);
        int minDistance = 25;
        while(Vector2.Distance(myCar.transform.position, DropPoints[randomDropPointID].transform.position) < minDistance)
        {
            randomDropPointID = Random.Range(0, DropPoints.Count);
            minDistance++;
        }
        var dropPoint = DropPoints[randomDropPointID];

        var SpawnedClient = PhotonNetwork.Instantiate("NPC" , spawnPoint.transform.position , Quaternion.identity).GetComponent<Client>();

        PhotonView clientPV = SpawnedClient.GetComponent<PhotonView>();
        clientPV.RPC("EnableThisClient", RpcTarget.Others, orignalSpawnId, randomDropPointID);

        SpawnedClient._SpawnPoint = spawnPoint;
        SpawnedClient._DropPoint = dropPoint;
        SpawnedClient.temp_dropID = randomDropPointID;
        SpawnedClient.temp_spawnID = randomSpawnPointID;


        spawnPoint.occupied = true;

        spawnPoint.myClient = SpawnedClient;
        SpawnedClient.isInialized = true;
        //dropPoint.myClients.Add(SpawnedClient);



        
        SpawnedClient.EnableClient();
    }
}

public enum CAMERA_TYPE
{
    CAR,PLAYER,MAP
}