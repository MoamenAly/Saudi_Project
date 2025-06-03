using BNG;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class RestOnUnGrab : MonoBehaviour
{
    internal Vector3 defaultPosition;
    Vector3 defaultUp;
    Quaternion defaultRoation;

    Vector3 intialPostion;

    [SerializeField] bool overrideTransform = false;
    [ShowIf(nameof(overrideTransform))][SerializeField] internal Transform overrideedTransform;


    LayerMask OnhandLayer = 1 << 7;
    LayerMask FreeLayer = 1 << 6;

    CustomGrabbable grabbable;
    bool grabed = false;
    bool attempGrab; //in remote grabing
    Rigidbody _rigidbody;

    public UnityEvent onGrab;
    public UnityEvent onUngrab;

    public ResetState ResetState;


    [SerializeField] bool shouldRest = false;
    [ShowIf(nameof(shouldRest))]


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 1;
        _rigidbody.drag = 1;

        defaultPosition = transform.localPosition;
        defaultRoation = transform.localRotation;
    }


    private void Start()
    {
        defaultPosition = transform.position;
        defaultRoation = transform.rotation;
        defaultUp = transform.up;

        intialPostion = transform.position;

        grabbable = GetComponent<CustomGrabbable>();

        if (grabbable != null)
        {
            grabbable.OnReleaseAction += OnUnGrab;
            grabbable.OnGrabAction += OnGrab;
        }

        //gameObject.layer = LayerMaskToLayer(FreeLayer);

    }

    private void OnGrab(Grabber grabber)
    {
        grabed = true;
        onGrab.Invoke();
    }

    private void OnUnGrab()
    {
        CancelInvoke(nameof(BackToDefaultPostion));
        grabed = false;
        Invoke(nameof(BackToDefaultPostion), 1.5f);
    }



    private void Update()
    {

        if (grabbable && attempGrab != grabbable.RemoteGrabbing && !grabbable.BeingHeld)
        {
            OnUnGrab();
        }

        //attemp granbed
        attempGrab = grabbable.RemoteGrabbing;
    }


    internal void BackToDefaultPostion()
    {
        if (shouldRest)
        {
            defaultPosition = intialPostion;
            transform.rotation = defaultRoation;
            transform.position = defaultPosition + Vector3.up * 0.01f;
        }
        else
        {
            if (ResetState == ResetState.Dynamic)
            {
                //Debug.Log("back to default" + gameObject.name);
                if (grabbable == null || grabbable.enabled == false || grabbable.BeingHeld) return;
                float diffenceInY = transform.position.y - defaultPosition.y;

                if (Mathf.Abs(diffenceInY) < 0.05)
                {
                    float y = defaultPosition.y;
                    defaultPosition = transform.position;
                    defaultPosition.y = y;
                    transform.position = defaultPosition;

                    if (Vector3.Dot(transform.up, Vector3.up) != 1)
                    {
                        transform.rotation = defaultRoation;
                    }

                    return;
                }
                else
                {
                    if (grabbable == null || grabbable.enabled == false || grabbable.BeingHeld) return;
                    transform.localPosition = defaultPosition;
                    transform.localRotation = defaultRoation;
                }
            }
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            if (overrideedTransform != null)
            {
                transform.rotation = defaultRoation;
                transform.position = overrideedTransform.position;
            }
            else
            {
                transform.rotation = defaultRoation;
                transform.position = defaultPosition + Vector3.up * 0.01f;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabed)
        {
            return;
        }
        else
        {
            OnUnGrab();
        }
    }

    //--------------------- Added for quiz -----------------------//
    public void ForceDefaultPosition()
    {
        Invoke(nameof(OnForceDefaultPosition), 1.5f);
    }
    public void OnForceDefaultPosition()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        if (overrideedTransform != null)
        {
            transform.rotation = defaultRoation;
            defaultPosition = transform.position = overrideedTransform.position;
        }
        else
        {
            transform.rotation = defaultRoation;
            transform.position = defaultPosition + Vector3.up * 0.01f;
        }
    }

}

public enum ResetState
{
    Fixed,
    Dynamic
}