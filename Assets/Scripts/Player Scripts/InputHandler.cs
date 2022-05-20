using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject actor;
    Animator anim;

    Command keyQ, keyE, keyW, idleCmd;

    CharacterController charController;
    CollisionFlags collisionFlags = CollisionFlags.None;

    float moveSpeed = 5f;
    float distFromPlayerToPointClicked;
    float gravity = 9.8f;
    float height;
    bool canMove;

    Vector3 targetPos = Vector3.zero;
    Vector3 playerMoveVec = Vector3.zero;

    List<Command> oldCommands = new List<Command>();
    Coroutine replayCoroutine;
    bool shouldStartReplay;
    bool isReplaying;


    void Awake()
    {
        keyQ = new AttackCommand();
        keyE = new TauntCommand();
        keyW = new MoveForwardCommand();
        idleCmd = new IdleCommand();
        anim = actor.GetComponent<Animator>();
        charController = actor.GetComponent<CharacterController>();
        Camera.main.GetComponent<CameraFollow>().player = actor.transform;
    }

    void Update()
    {
        if (!isReplaying) HandleInput();
        MovePlayer();
    }

    

    void UndoLastCommand()
    {
        var lastIdx = oldCommands.Count - 1;
        if (lastIdx < 0) return;
        var cmd = oldCommands[lastIdx];
        cmd.Execute(anim);
        oldCommands.RemoveAt(lastIdx);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            keyQ.Execute(anim);
            oldCommands.Add(keyQ);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            keyE.Execute(anim);
            oldCommands.Add(keyE);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            keyW.Execute(anim);
            oldCommands.Add(keyW);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            StartReplay();

        if (Input.GetKeyDown(KeyCode.Z))
            UndoLastCommand();
    }

    void StartReplay()
    {
        if (oldCommands.Count > 0)
        {
            if (replayCoroutine != null)
            {
                StopCoroutine(replayCoroutine);
            }
            replayCoroutine =   StartCoroutine(ReplayCommands());
        }
    }

    IEnumerator ReplayCommands()
    {
        isReplaying = true;
        
        for (int i = 0; i < oldCommands.Count; ++i )
        {
            var cmd = oldCommands[i];
            cmd.Execute(anim);
            yield return new WaitForSeconds(1f);
        }

        //oldCommands.Clear();
        isReplaying = false;
    }

    /*IEnumerator ReplayCommands()
    {
        isReplaying = true;
        int i = 0;
        foreach (var cmd in oldCommands)
        {
            cmd.Execute(anim);
            //oldCommands.RemoveAt(i++);
            yield return new WaitForSeconds(1f);
        }
        isReplaying = false;
    }*/

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
        CalcMoveVecAndRotate();
        collisionFlags = charController.Move(playerMoveVec);

        print(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
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
                    distFromPlayerToPointClicked = Vector3.Distance(hit.point, actor.transform.position);
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
            var targetTmp = new Vector3(targetPos.x, actor.transform.position.y, targetPos.z);
            actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, Quaternion.LookRotation(targetTmp - actor.transform.position), 15.0f * Time.deltaTime);
            playerMoveVec = actor.transform.forward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(targetTmp, actor.transform.position) <= 0.5f)
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
