using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour {



    // [SerializeField] bool m_isAiming;
    [SerializeField] bool m_isIdle;
    [SerializeField] bool m_isCrouched;
    [SerializeField] bool m_isOnCover;
    [SerializeField] bool m_hitLeft;
    [SerializeField] bool m_hitRight;
    [SerializeField] bool m_hitTop;

    [SerializeField] float m_TurnSpeed = 4.0f;
    [SerializeField] float m_smoothRot = 1f;
    [SerializeField] float m_walkSpeed = 8;
    //[SerializeField] float m_runSpeed = 9;
    [SerializeField] float m_crouchSpeed = 7;
    [SerializeField] float m_currentSpeed = 8;

    [SerializeField] float m_horizontalMovement;
    [SerializeField] float m_verticalMovement;

    [SerializeField] float m_horizontalMouse;
    [SerializeField] Camera m_mainCamera;

    [SerializeField] int m_layerMask;
    [SerializeField] float m_coverRange=5.0f;

    [SerializeField] Transform m_leftRayCastPoint;
    [SerializeField] Transform m_RightRayCastPoint;
    [SerializeField] Transform m_TopRayCastPoint;
    [SerializeField] Transform m_MidRayCastPoint;

    [SerializeField] Vector3 m_currentCover;


    [SerializeField] Animator anim;
    [SerializeField] GameObject m_Character;
    [SerializeField] Vector3 m_position;


    void Start () {
        anim = GetComponentInChildren<Animator>();
        m_mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        MoveCharacter();
        if(!m_isOnCover)
            RotateCharacter();
        CheckCover();

        if (Mathf.Abs(m_verticalMovement)<0.1 && Mathf.Abs(m_horizontalMovement) < 0.1)
        {
            m_isIdle = true;
            anim.SetBool("isIdle", true);
        }
        else
        {
            m_isIdle = false;
            anim.SetBool("isIdle", false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            m_isCrouched = !m_isCrouched;
            anim.SetBool("isCrouched", m_isCrouched);
        }

        

    }
    public void CheckCover()
    {
        RaycastHit hit;

        //Left Raycast
        if (Physics.Raycast(m_leftRayCastPoint.transform.position, m_leftRayCastPoint.transform.TransformDirection(Vector3.forward), out hit, m_coverRange))
        {
            m_hitLeft = true;
           
        }
        else
        {
            m_hitLeft = false;
        }
        //Right Raycast
        if (Physics.Raycast(m_RightRayCastPoint.transform.position, m_RightRayCastPoint.transform.TransformDirection(Vector3.forward), out hit, m_coverRange))
        {
            m_hitRight = true;
            

        }
        else
        {
            m_hitRight = false;
        }
        //Top Raycast
        if (Physics.Raycast(m_TopRayCastPoint.transform.position, m_TopRayCastPoint.transform.TransformDirection(Vector3.forward), out hit, m_coverRange))
        {
            
            m_hitTop = true;
        }
        else
        {
            m_hitTop = false;
        }
        //center raycast used to keep player a set distance from the cover
        if (Physics.Raycast(m_MidRayCastPoint.transform.position, m_MidRayCastPoint.transform.TransformDirection(Vector3.forward), out hit, m_coverRange))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (m_currentCover != hit.point && m_isOnCover)
            {
                Vector3 temp = new Vector3(m_currentCover.x, transform.position.y, m_currentCover.z);
                transform.position = (transform.position - temp).normalized * 0.5f + temp;
                m_currentCover = hit.point;
               
            }
               
            
        }
        else
        {
            //error?
        }
        //Crouching Cover

        //toggle crouch and cover animation
        if (m_hitLeft && m_hitRight && !m_hitTop )
        {
            // can only enter cover when both rays hit
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!m_isOnCover)
                {
                    m_isCrouched = true;
                    anim.SetBool("isCrouched", true);
                    m_isOnCover = true;
                    anim.SetBool("isInCover", true);
                    //rotate model
                    Quaternion wantedRotation = Quaternion.LookRotation(hit.normal);
                    m_Character.transform.rotation = wantedRotation;

                    //var targetAngle = transform.eulerAngles + 180f * Vector3.up;
                    // m_Character.transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetAngle, m_smoothRot * Time.deltaTime);


                }
                else
                {
                    m_isCrouched = false;
                    anim.SetBool("isCrouched", false);
                    m_isOnCover = false;
                    anim.SetBool("isInCover", false);
                    Quaternion wantedRotation = Quaternion.LookRotation(-1*hit.normal);
                    m_Character.transform.rotation = wantedRotation;
                }
            }
        }
        //Add actions for moving to another cover or jumping from cover to cover

        // I am at the end of the left side
        else if (m_hitLeft && !m_hitRight && !m_hitTop)
        {
            //turn on left peaking animation
            
        }
        // I am at the end of the right side
        else if (!m_hitLeft && m_hitRight && !m_hitTop)
        {
            //turn on right peaking animation
        }

        //Standing Cover
        // I am not at the ends
        if (m_hitLeft && m_hitRight && m_hitTop)
        {
            //turn off peaking animation if on
        }
        // I am at the end of the left side
        else if (m_hitLeft && !m_hitRight && m_hitTop)
        {
            //turn on left peaking animation
        }
        // I am at the end of the right side
        else if (!m_hitLeft && m_hitRight && m_hitTop)
        {
            //turn on right peaking animation
        }

        
    }



    public void MoveCharacter()
    {
        m_horizontalMovement = Input.GetAxis("Horizontal");
        m_verticalMovement = Input.GetAxis("Vertical");
        anim.SetFloat("Horizontal", m_horizontalMovement);
        anim.SetFloat("Vertical", m_verticalMovement);

        if (!m_isOnCover)
        {
            Vector3 Movement = new Vector3(m_horizontalMovement, 0, m_verticalMovement);
            gameObject.transform.position += Movement * m_currentSpeed * Time.deltaTime;
        }
        else if(m_isOnCover && !m_hitLeft && m_horizontalMovement<-0.1)
        {
            Vector3 Movement = new Vector3(m_horizontalMovement, 0, 0);
            gameObject.transform.position += Movement * m_currentSpeed * Time.deltaTime;
        }
        else if (m_isOnCover && !m_hitRight && m_horizontalMovement > 0.1)
        {
            Vector3 Movement = new Vector3(m_horizontalMovement, 0, 0);
            gameObject.transform.position += Movement * m_currentSpeed * Time.deltaTime;
        }
        else if (m_isOnCover && m_hitRight && m_hitLeft)
        {
            Vector3 Movement = new Vector3(m_horizontalMovement, 0, 0);
            gameObject.transform.position += Movement * m_currentSpeed * Time.deltaTime;
        }


    }
    public void RotateCharacter()
    {
        if (m_isOnCover)
            return;

        //This should effect animations need to add state/direction parameter to anim

        float mouseHorizontal = Input.GetAxis("Mouse X");
        m_horizontalMouse = (m_horizontalMouse + m_TurnSpeed * mouseHorizontal) % 360f;
        transform.rotation = Quaternion.AngleAxis(m_horizontalMouse, Vector3.up);
    }

}
