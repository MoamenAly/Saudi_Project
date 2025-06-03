using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrabbable : Grabbable
{
    public Action<Grabber> OnGrabAction;
    public Action OnReleaseAction;

    //[SerializeField] LayerMask _handOffLayer;
    //[SerializeField] LayerMask _handOnLayer;

    /*[SerializeField]*/ int handOffLayer;
   /* [SerializeField]*/ int handOnLayer;

    bool collisionEnabled = true;

    private void Start()
    {
        handOffLayer = LayerMask.NameToLayer("Grabbale");
        handOnLayer  = LayerMask.NameToLayer("OnHand_Grabbale"); 
    }

    public override void GrabItem(Grabber grabbedBy)
    {
        base.GrabItem(grabbedBy);
        OnGrabAction?.Invoke(grabbedBy);
        gameObject.layer = handOnLayer; //LayerMaskToLayer(_handOnLayer);
        Invoke(nameof(UpdatehandPose),0.1f);   
    }

    private void UpdatehandPose()
    {
        //if (primaryGrabOffset != null && ActiveGrabPoint != null)
        //{
        //    Debug.Log(gameObject.name + "  set hand poses to hand");
        //    CustomHandPose = primaryGrabOffset.GetComponent<GrabPoint>().HandPose;
        //    SelectedHandPose = primaryGrabOffset.GetComponent<GrabPoint>().SelectedHandPose;
        //    handPoseType = primaryGrabOffset.GetComponent<GrabPoint>().handPoseType;
        //}
        //else
        //{        
        //    CustomHandPose = initialHandPoseId;
        //    SelectedHandPose = initialHandPose;
        //    handPoseType = initialHandPoseType;
        //}
    }

    public override void Update()
    {
        base.Update();

        if (collisionEnabled)
        {
            if (remoteGrabbing)
            {
                gameObject.layer = handOnLayer; //LayerMaskToLayer(_handOnLayer);
            }
            else
            {
                gameObject.layer = handOffLayer;//LayerMaskToLayer(_handOffLayer);
            }
        }
        else if (BeingHeld)
        {
            gameObject.layer = handOnLayer;//LayerMaskToLayer(_handOnLayer);
        }

    }

    internal void _EnableCollison()
    {
        collisionEnabled = true;
    }

    internal void _DisableCollsion()
    {
        collisionEnabled = false;
    }



    public override void DropItem(Grabber droppedBy, bool resetVelocity, bool resetParent)
    {
        bool BeingHeldCopy = BeingHeld;

        base.DropItem(droppedBy, resetVelocity, resetParent);

        if (BeingHeldCopy == true)
        {
            OnReleaseAction?.Invoke();
            gameObject.layer = handOffLayer; //LayerMaskToLayer(_handOffLayer);
        }
    }

    // Add this method to manually adjust the rotation during FixedUpdate

   

  /*  public override void GrabItem(Grabber grabbedBy)
    {

        // Make sure we release this item
        if (BeingHeld && SecondaryGrabBehavior != OtherGrabBehavior.DualGrab)
        {
            DropItem(false, true);
        }

        bool isPrimaryGrab = !BeingHeld;
        bool isSecondaryGrab = BeingHeld && SecondaryGrabBehavior == OtherGrabBehavior.DualGrab;

        // Officially being held
        BeingHeld = true;
        LastGrabTime = Time.time;

        // Primary Grabber just grabbed this item
                Debug.Log(gameObject.name + "  isPrimaryGrab  " + isPrimaryGrab);
        if (isPrimaryGrab)
        {
            // Make sure all values are reset first
            ResetGrabbing();

            // Set where the item will move to on the grabber
            primaryGrabOffset = GetClosestGrabPoint(grabbedBy);
            secondaryGrabOffset = null;

            // Set the active Grab Point that we will be using
            if (primaryGrabOffset)
            {
                ActiveGrabPoint = primaryGrabOffset.GetComponent<GrabPoint>();
            } 
            else
            {
                ActiveGrabPoint = null;
            }
                Debug.Log(gameObject.name + "  primaryGrabOffset  " + primaryGrabOffset);

            // Update Hand Pose Id
            if (primaryGrabOffset != null && ActiveGrabPoint != null)
            {
                Debug.Log(gameObject.name + "  set hand poses to hand");
                CustomHandPose = primaryGrabOffset.GetComponent<GrabPoint>().HandPose;
                SelectedHandPose = primaryGrabOffset.GetComponent<GrabPoint>().SelectedHandPose;
                handPoseType = primaryGrabOffset.GetComponent<GrabPoint>().handPoseType;
            }
            else
            {
                Debug.Log(gameObject.name + "  set hand poses to hand");
                Debug.Log(gameObject.name + "  set hand poses  ");
                CustomHandPose = initialHandPoseId;
                SelectedHandPose = initialHandPose;
                handPoseType = initialHandPoseType;
            }

            // Update held by properties
            addGrabber(grabbedBy);
            grabTransform.parent = grabbedBy.transform;
            rotateGrabber(false);

            // Use center of grabber if snapping
            if (GrabMechanic == GrabType.Snap)
            {
                grabTransform.localEulerAngles = Vector3.zero;
                grabTransform.localPosition = -GrabPositionOffset;
            }
            // Precision hold can use position of what we're grabbing
            else if (GrabMechanic == GrabType.Precise)
            {
                grabTransform.position = transform.position;
                grabTransform.rotation = transform.rotation;
            }

            // First remove any connected joints if necessary
            var projectile = GetComponent<Projectile>();
            if (projectile)
            {
                var fj = GetComponent<FixedJoint>();
                if (fj)
                {
                    Destroy(fj);
                }
            }

            // Setup any relevant joints or required components
            if (GrabPhysics == GrabPhysics.PhysicsJoint)
            {
                setupConfigJointGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.Velocity)
            {
                setupVelocityGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.FixedJoint)
            {
                //setupConfigJointGrab(grabbedBy, GrabMechanic);

                setupFixedJointGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.Kinematic)
            {
                setupKinematicGrab(grabbedBy, GrabMechanic);
            }

            // Stop our object on initial grab
            if (rigid && !rigid.isKinematic)
            {


                SetRigidVelocity(Vector3.zero);
                SetRigidAngularVelocity(Vector3.zero);
            }

            // Let events know we were grabbed
            for (int x = 0; x < events.Count; x++)
            {
                events[x].OnGrab(grabbedBy);
            }

            checkParentHands(grabbedBy);

            // Move Hand Model
            if (GrabMechanic == GrabType.Precise && SnapHandModel && primaryGrabOffset != null && grabbedBy.HandsGraphics != null)
            {
                grabbedBy.HandsGraphics.transform.parent = primaryGrabOffset;
                grabbedBy.HandsGraphics.localPosition = grabbedBy.handsGraphicsGrabberOffset;
                grabbedBy.HandsGraphics.localEulerAngles = grabbedBy.handsGraphicsGrabberOffsetRotation;
            }

            SubscribeToMoveEvents();

        }
        else if (isSecondaryGrab)
        {
            // Set where the item will move to on the grabber
            secondaryGrabOffset = GetClosestGrabPoint(grabbedBy);

            // Update held by properties
            addGrabber(grabbedBy);

            grabTransformSecondary.parent = grabbedBy.transform;

            // Use center of grabber if snapping
            if (GrabMechanic == GrabType.Snap)
            {
                grabTransformSecondary.localEulerAngles = Vector3.zero;
                grabTransformSecondary.localPosition = GrabPositionOffset;
            }
            // Precision hold can use position of what we're grabbing
            else if (GrabMechanic == GrabType.Precise)
            {
                grabTransformSecondary.position = transform.position;
                grabTransformSecondary.rotation = transform.rotation;
            }

            checkParentHands(grabbedBy);

            // Move Hand Model if snap hands and precise
            if (GrabMechanic == GrabType.Precise && SnapHandModel && secondaryGrabOffset != null && grabbedBy.HandsGraphics != null)
            {
                grabbedBy.HandsGraphics.transform.parent = secondaryGrabOffset;
                grabbedBy.HandsGraphics.localPosition = grabbedBy.handsGraphicsGrabberOffset;
                grabbedBy.HandsGraphics.localEulerAngles = grabbedBy.handsGraphicsGrabberOffsetRotation;
            }
        }

        // Hide the hand graphics if necessary
        if (HideHandGraphics)
        {
            grabbedBy.HideHandGraphics();
        }

        journeyLength = Vector3.Distance(grabPosition, grabbedBy.transform.position);

        OnGrabAction?.Invoke(grabbedBy);
    }

    */   

}
