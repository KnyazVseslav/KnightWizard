using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //private GameObject actor;

    private Animator anim;
    private CharacterController charController;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    private float moveSpeed = 5f;
    private float distFromPlayerToPointClicked;
    private float gravity = 9.8f;
    private float height;
    private bool canMove;
    //private bool moveFinished = true;

    private Vector3 targetPos = Vector3.zero;
    private Vector3 playerMoveVec = Vector3.zero;


    void Awake()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        //actor = GameObject.FindGameObjectWithTag("InputHandler").GetComponent<InputHandler>().actor;
        //anim = actor.GetComponent<Animator>();
        //charController = actor.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    bool IsGrounded()
    {
        return collisionFlags == CollisionFlags.CollidedBelow ? true : false;
    }

    void ApplyGravityDeltaY(ref Vector3 moveVec)
    {
        if (IsGrounded()) height = 0f;
        else height -= gravity * Time.deltaTime;

        moveVec.y = height * Time.deltaTime;
    }

    void MovePlayer()
    {
        /*CalcMoveVecAndRotate();
        collisionFlags = charController.Move(playerMoveVec);

        print("IsName Stand = " + anim.GetCurrentAnimatorStateInfo(0).IsName("Stand"));
        print("!anim.IsInTransition(0) = " + !anim.IsInTransition(0));
        print("anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f   = " + (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f));*/

        CalcMoveVecAndRotate();
        collisionFlags = charController.Move(playerMoveVec);

        print(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        /*if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            print("FINISHED MOVE");
        }
        else
        {
            print("MOVING");
        }*/

        /* if (!moveFinished)
         {
             print("if (!moveFinished)");

             if (!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand") &&
                 anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
             {
                 moveFinished = true;
                 print("FINISHED MOVE");
             }
         }
         else
         {
             print("MOVING");

             CalcMoveVecAndRotate();
             collisionFlags = charController.Move(playerMoveVec);
         }*/
    }

    void CalcMoveVecAndRotate()
    {
        ApplyGravityDeltaY(ref playerMoveVec);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {
                    distFromPlayerToPointClicked = Vector3.Distance(hit.point, transform.position);
                    if (distFromPlayerToPointClicked >= 1.0f)
                    {
                        targetPos = hit.point;
                        canMove = true;
                    }
                }
            }
        }

        if (canMove)
        {
            anim.SetFloat("Walk", 1.0f);
            var targetTmp = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetTmp - transform.position), 15.0f * Time.deltaTime);
            playerMoveVec = transform.forward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(targetTmp, transform.position) <= 0.5f)
            {
                canMove = false;
            }
        }
        else
        {
            playerMoveVec.Set(0, 0, 0);
            anim.SetFloat("Walk", 0);
        }
    }
}
