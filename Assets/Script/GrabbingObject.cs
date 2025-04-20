using System;using UnityEngine;

public class GrabbingObject : MonoBehaviour
{
    public GameObject heldObject;
    public Transform grabPoint; // empty transform on the hand where the object will attach
    public UDPReceive udpReceive;
    private bool grabbing = false;
    [SerializeField]private MountRotation mountRotation;
    private Rigidbody rb;
   private Rigidbody rb_mount;
    [SerializeField] private GameObject mount;
    private float xRot;
    private void Start()
    {
         xRot = mount.transform.eulerAngles.x;
         rb = heldObject.GetComponent<Rigidbody>();
         rb_mount = mount.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (udpReceive == null) return;

       // Normalize here
        string data = udpReceive.receivedData;
        Debug.Log($"Gesture Data: {data}, Mount X: {xRot}");

        if (data == "Open Hand" && !grabbing)
        {
            mountRotation.enabled = true;
            Operating();
            grabbing = true;
        }
        else if ((data == "Close Hand") && grabbing)
        {
            mountRotation.enabled = true;
            Debug.Log("Close Hand detected - releasing object");
            ReverseOperating();
            //ReleaseObject();
            grabbing = false;
        }
        else
        {
            ReleaseObject();
        }

        if (xRot < 90f && xRot > 125f)
        {
            rb_mount.freezeRotation = true;
        }


    }


    private void OnTriggerEnter(Collider other)
    {
        if (heldObject == null && other.CompareTag("Grabbable"))
        {
            GrabObject(other.gameObject);
        }
    }

    public void GrabObject(GameObject obj)
    {
        heldObject = obj;
        rb = heldObject.GetComponent<Rigidbody>(); // Move this here

        heldObject.transform.SetParent(grabPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    
        rb.useGravity = false;
        rb.isKinematic = true;
    }


    public void Operating()
    {
        //rb_mount.freezeRotation = false;
        mountRotation.StartForwardRotation();
    }

    public void ReverseOperating()
    {
        mountRotation.StartReverseRotation();
        ReleaseObject();
    }


    public void ReleaseObject()
    {
        if (heldObject != null)
        {
            Debug.Log("Releasing object");
            heldObject.transform.SetParent(null);
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            heldObject = null;
        }
        else
        {
            Debug.LogWarning("No object to release!");
        }
    }


}