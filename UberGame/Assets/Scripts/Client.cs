using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{

    public int PassengerAmount = 1;
    public bool AllPickedUp = false;
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
                    CommonReferences.Instance.myCar.pickedUpClients.Add(this);
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
    }

    public void SendInCar(Transform car)
    {
        for (int i = 0; i < PassengerAmount; i++)
        {
            int temp = i;
            passengers[temp].GetInCar(car);
        }
    }
}
