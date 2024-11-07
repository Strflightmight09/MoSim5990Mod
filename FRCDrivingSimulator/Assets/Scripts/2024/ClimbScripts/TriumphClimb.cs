using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TriumphClimb : MonoBehaviour, IResettable
{
    [SerializeField] private ConfigurableJoint climber;
    [SerializeField] private ConfigurableJoint hooks;
    [SerializeField] private HingeJoint trap;

    private TriumphAmpArm ampArm;

    private int startingLayer;

    private bool climb;
    private bool hang;
    private bool prepped;
    private bool isClimbing;

    private Vector3 climberStartingPos;
    private Quaternion climberStartingRot;
    private Vector3 hooksStartingPos;
    private Quaternion hooksStartingRot;
    private Vector3 trapStartingPos;
    private Quaternion trapStartingRot;
    
    private void Start() 
    {
        climberStartingPos = climber.gameObject.transform.localPosition;
        climberStartingRot = climber.gameObject.transform.localRotation;

        hooksStartingPos = hooks.gameObject.transform.localPosition;
        hooksStartingRot = hooks.gameObject.transform.localRotation;

        trapStartingPos = trap.gameObject.transform.localPosition;
        trapStartingRot = trap.gameObject.transform.localRotation;

        startingLayer = climber.gameObject.layer;

        ampArm = GetComponent<TriumphAmpArm>();
    }

    private void Update()
    {
        if (climb && !isClimbing)
        {
            isClimbing = true;
            StartCoroutine(ClimbSequence());
        }
        else if (hang && prepped)
        {
            prepped = false;
            HangSequence();
            ampArm.TrapAmpArm();
        }
    }

    private IEnumerator ClimbSequence() 
    {
        climber.targetPosition = new Vector3(0f, -3.1f, 0f);
        hooks.targetPosition = new Vector3(0f, -3.9f, 0f);
        JointSpring forksSpring = trap.spring;
        forksSpring.targetPosition = -90f;
        trap.spring = forksSpring;
        yield return new WaitForSeconds(0.5f);
        prepped = true;
    }

    private void HangSequence() 
    {
        climber.targetPosition = new Vector3(0f, 0f, 0f);
        hooks.targetPosition = new Vector3(0f, 0f, 0f);
    }

    public void OnClimb(InputAction.CallbackContext ctx)
    {
        climb = ctx.action.triggered;
    }

    public void OnHang(InputAction.CallbackContext ctx)
    {
        hang = ctx.action.triggered;
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        climber.gameObject.layer = startingLayer;
        trap.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        climber.gameObject.layer = 17;
        trap.gameObject.layer = 17;

        prepped = false;
        isClimbing = false;

        //Reset joints pos and rot and targetPos
        climber.gameObject.transform.localPosition = climberStartingPos;
        climber.gameObject.transform.localRotation = climberStartingRot;

        hooks.gameObject.transform.localPosition = hooksStartingPos;
        hooks.gameObject.transform.localRotation = hooksStartingRot;

        trap.gameObject.transform.localPosition = trapStartingPos;
        trap.gameObject.transform.localRotation = trapStartingRot;

        climber.targetPosition = new Vector3(0f, 0f, 0f);
        hooks.targetPosition = new Vector3(0f, 0f, 0f);
        
        JointSpring forksSpring = trap.spring;
        forksSpring.targetPosition = 0f;
        trap.spring = forksSpring;
        StartCoroutine(WaitToEnable());
    }
}
