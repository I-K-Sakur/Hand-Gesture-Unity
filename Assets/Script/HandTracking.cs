
using UnityEngine;
using UnityEngine.UIElements;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    [SerializeField] private GameObject handObject;
    public GrabbingObject grabbingObject;
    private bool grabbing = false;
    private void Update()
    {
        string data = udpReceive.receivedData;
      
        print(data);

        if (data == "Open Hand")
        {
            grabbingObject.GrabObject(null);
            grabbing = true;
        }
        else if (grabbing && (data == "Unknown" || data == "Close Hand"))
        {
            grabbingObject.ReleaseObject();
            grabbing = false;
        }
    }
}
