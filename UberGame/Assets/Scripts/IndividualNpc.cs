using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualNpc : MonoBehaviour
{
    [SerializeField] float xBounds;
    [SerializeField] float yBounds;
    [SerializeField] List<Sprite> npcTypes = new List<Sprite>();
    [SerializeField] SpriteRenderer mySprite;
    private Vector3 myInitPos;
   /* [SerializeField] Collider2D myCollider;
    private PlayerInput myInput;
    private bool Dead;*/

    private bool _isPickedUp =false;
    public bool isPickedUp { get { return _isPickedUp; }
        set
        {
            _isPickedUp = value;
            if (isPickedUp)
            {
                if (myClient != null)
                {
                    myClient.PassengersPickedup++;
                }
            }
        }
    }
    private Client myClient;
    public void Initialize(int ID)
    {
        mySprite = GetComponent<SpriteRenderer>();
        myClient = GetComponentInParent<Client>();
        /*myCollider = GetComponent<Collider2D>();
        myInput = CommonReferences.Instance.myPlayer._input;*/


        float randomx = Random.Range(-xBounds, xBounds);
        float randomy = Random.Range(-yBounds, yBounds);
        int randomPic = Random.Range(0, npcTypes.Count);

        myInitPos = new Vector3(randomx, randomy);
        if (ID != 0)
            transform.localPosition = myInitPos;

        mySprite.sprite = npcTypes[randomPic];
        this.gameObject.SetActive(true);
    }

    public void GetInCar(Transform car)
    {
        StartCoroutine(GetInCarCO(car));
    }

    IEnumerator GetInCarCO(Transform car)
    {
        /* while(myCollider.enabled)
         {
             if (Mathf.Abs(myInput.GetHorizontalInput()) == 0 && Mathf.Abs(myInput.GetVerticalInput()) == 0)
             {
                 transform.position = Vector2.MoveTowards(transform.position, car.position, Time.deltaTime * 3);
             }
             else
             {
                 transform.position = Vector2.MoveTowards(transform.position, myInitPos, Time.deltaTime * 3);
             }
             yield return new WaitForEndOfFrame();
         }*/
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
        isPickedUp = true;
        myClient._SpawnPoint.occupied = false;
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponentInParent<PhotonView>() == null) return;

        if (collision.transform.GetComponentInParent<PhotonView>().IsMine && (collision.transform.CompareTag("Player") || collision.transform.CompareTag("car")))
        {
            if ((Mathf.Abs(myInput.GetHorizontalInput()) == 0 && Mathf.Abs(myInput.GetVerticalInput()) == 0))
            {
                myCollider.enabled = false;
            }
            else
            {

            }
        }
    }*/
}
