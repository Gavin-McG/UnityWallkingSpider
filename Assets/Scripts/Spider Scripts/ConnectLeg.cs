using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ConnectLeg : MonoBehaviour
{
    //Create and connect legs and knees of two objects

    //order of tether and target doesnt matter but helps give distinction
    [SerializeField] GameObject tether; //object which is the center
    public GameObject target; //object which is connected to the tether

    //prefabs used as the legs (should be 1x1x1 to make scaling accurate)
    [SerializeField] GameObject legPrefab;
    [SerializeField] GameObject kneePrefab;
    
    public float legLength; //length of each leg segment
    public float diameter; //width of leg
    public bool hasKnee; //whether to show thee knee
    public bool isRight;

    public float upwardsRotation;

    //gameobjects that make up leg
    private GameObject leg1;
    private GameObject leg2;
    private GameObject knee;

    // Start is called before the first frame update
    void Start()
    {
        //instantiate leg pieces
        leg1 = Instantiate(legPrefab, transform);
        leg2 = Instantiate(legPrefab, transform);
        knee = Instantiate(kneePrefab, transform);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //update scale of legs to match leglength and diameter
        leg1.transform.localScale = new Vector3(diameter, diameter, legLength);
        leg2.transform.localScale = new Vector3(diameter, diameter, legLength);
        knee.transform.localScale = Vector3.one * diameter;

        //get the distamce between the tether and the target
        Vector3 seperation = target.transform.position - tether.transform.position;
        if (seperation.magnitude>legLength*2)
        {
            //limit seperation objects to the reachof the leg
            seperation = seperation.normalized * legLength * 2;
            target.transform.position = tether.transform.position + seperation;
        }

        //calculate upwards direction that the knee should be using this object's tranform
        Vector3 upwards = Quaternion.AngleAxis(upwardsRotation, seperation) * -Vector3.Cross(Vector3.Cross(transform.up, seperation), seperation).normalized;

        //height of the knee in order to make leg lengths consistent
        float liftDist = Mathf.Sqrt(Mathf.Max(Mathf.Pow(legLength, 2) - Mathf.Pow(seperation.magnitude / 2, 2), 0));

        //position of the knee
        Vector3 kneePoint = tether.transform.position + seperation / 2 + liftDist * upwards;

        //place legs between each object and the knee
        leg1.transform.position = (kneePoint + tether.transform.position) / 2;
        leg2.transform.position = (kneePoint + target.transform.position) / 2;

        //orient legs
        leg1.transform.rotation = Quaternion.LookRotation(leg1.transform.position - kneePoint, transform.up);
        leg2.transform.rotation = Quaternion.LookRotation(leg2.transform.position - kneePoint, transform.up);

        //setand orient knee
        knee.transform.position = kneePoint;
        knee.transform.rotation = Quaternion.LookRotation(leg1.transform.position - kneePoint, transform.up);

        //show or dont show knee
        knee.gameObject.SetActive(hasKnee);
    }
}
