using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    private Rigidbody myRigidbody;
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private Character player;
    private CapsuleCollider playerCollider;

    float velocity;
    public Vector3 direction;
    private Vector3 forward, right;


    private Vector3 targetPos = new Vector3(-1,-1,-1);
    public float time =1f;
    public bool canMove = true;

    private void Start() {
        player = GetComponent<Character>();
        playerInput = GetComponent<PlayerInput>();
        myRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);

        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

public void Move(float _velocity)
    {
        velocity = _velocity;
    }
    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);

        transform.LookAt(heightCorrectedPoint);
    }

    public void FixedUpdate()
    {
        /* RaycastHit hit;
         //direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
         //Vector3 moveinput = (myRigidbody.transform.right * playerInput.moveHorizontal + myRigidbody.transform.forward * playerInput.moveVertical).normalized;

         //Vector3 moveinput = myRigidbody.transform.forward * playerInput.moveVertical;
         //moveinput += Quaternion.Euler(0, 90, 0) * myRigidbody.transform.forward * playerInput.moveHorizontal;

         //velocity = playerInput.sprint ? sprintSpeed : moveSpeed;

         Vector3 rightMovement = right * Time.deltaTime * Input.GetAxis("Horizontal");
         Vector3 upMovement = forward * Time.deltaTime * Input.GetAxis("Vertical");
         Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

         velocity = player.GetVelocity() + player.UseFastSKill(heading);
         direction *= velocity * 0.2f;
         //Debug.DrawLine(myRigidbody.position, myRigidbody.position + Vector3.down * 0.1f, Color.blue);
         //if (Physics.Raycast(myRigidbody.position + Vector3.down * 1f, Vector3.down, out hit, 0.1f))
         //{
         //    var posY = myRigidbody.position;
         //    posY.y = hit.transform.position.y;

         //    myRigidbody.position = posY;
         //}


         // if(transform.localEulerAngles.y >= )

         playerAnimator.SetFloat("MoveHorizontal", direction.x);
         playerAnimator.SetFloat("MoveVertical", direction.z);

         //playerAnimator.SetFloat("Move", moveinput.x);
         //playerAnimator.SetFloat("Move", moveinput.z);

         //direction = heading;
         //myRigidbody.MovePosition(myRigidbody.position + heading * velocity * Time.deltaTime);

         // enemy 제외한 마스크 전부
         if (Physics.Raycast(myRigidbody.position, heading, 0.5f, ~(1 << LayerMask.GetMask("Enemy"))))
         {
             myRigidbody.velocity = Vector3.zero;
         }
         else
         {
             direction = heading;
             //myRigidbody.MovePosition(myRigidbody.position + heading * velocity * Time.deltaTime);
             myRigidbody.velocity = velocity * heading;
         }*/
        if(targetPos != new Vector3(-1, -1, -1) || Vector3.Distance(transform.position, targetPos) < 0.1f)
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 3 * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }


    public IEnumerator Moving(Vector3 pos)
    {
       
        Debug.LogError("Moving1: " + pos);
        Debug.LogError("Moving2: " + transform.position);

       
        //Debug.LogError("Moving3: " + Vector3.Lerp(pos, transform.position, 0.1f));
        while (true)
        {
            if (Vector3.Distance(transform.position, pos) < 0.1f )
            {
                
                yield break; 
            }


            transform.position = Vector3.MoveTowards(transform.position, pos, 10 * Time.deltaTime);


            yield return null;
        }

        

    }

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }


   

   
}