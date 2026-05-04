using AILogicGroup;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "AILogicGroundPatrol", menuName = "AILogic/Ground Patrol")]
public class AILogicGroundPatrol : AILogic
{
    public float AIMoveSpeed = 2f;
    [Header("Player Detection")]
    public float PlayerDetectionDistance = 20;
    public float PlayerForwardDetectionDistance = 10;
    public float PlayerMaxChaseDistance = 10;
    public LayerMask PlayerMask;
    public float playerHeightCheck = 2f;
    [Header("Chase settings")]
    public float stopDistance = 3f;
    public float attackPause = 2f;
    public float attackDuration = 2f;
    [Header("Patrol settings")]
    public float turnPauseDuration = 2f;


    [Header("Environment Detection")]
    public float GroundCheckDistance = 2f;
    public float GroundCheckDistanceFromBody = 2f;
    public LayerMask GroundMask;
    public int FrameCountToCheckForGround = 10;

    
    public override void OnEnterLogic(CharacterLogicHandler logicHandler)
    {
        GetBlackboard(logicHandler).Set<int>("facedirection", -1);
        GetBlackboard(logicHandler).Set<float>("movespeed", AIMoveSpeed);
        GetBlackboard(logicHandler).Set<bool>("playerdetected", false);
        StartPatrol(logicHandler);
    }
    public override void OnExitLogic(CharacterLogicHandler logicHandler)
    {
        base.OnExitLogic(logicHandler);
        logicHandler.StopAllCoroutines();
    }
    public override void FrameUpdate(CharacterLogicHandler logicHandler)
    {
        if (GetBlackboard(logicHandler).Get<GameObject>("target") == null)
        {
            FindTarget(logicHandler);
            return;
        }
        if (Time.frameCount % 30 != 0)
            return;
        
        if( PlayerForwardDetection(logicHandler) && PlayerSightCheck(logicHandler) && PlayerHeightCheck(logicHandler) && PlayerDistanceCheck(logicHandler) && GetBlackboard(logicHandler).Get<string>("currentstate") == "patrol" )
        {
            GetBlackboard(logicHandler).Set<bool>("playerdetected", true);
            StartChase(logicHandler);
        }
        if (GetBlackboard(logicHandler).Get<bool>("hittrigger") && GetBlackboard(logicHandler).Get<string>("currentstate") == "patrol")
        {
            GetBlackboard(logicHandler).Set<bool>("playerdetected", true);
            StartChase(logicHandler);
        }
        if (GetBlackboard(logicHandler).Get<bool>("isdead"))
        {
            logicHandler.DisableLogic();
        }
    }
    public override Blackboard GetBlackboard(CharacterLogicHandler logicHandler)
    {
        return logicHandler.blackboard;
    }
    IEnumerator PatrolState(CharacterLogicHandler logicHandler)
    {
        GetBlackboard(logicHandler).Set<string>("currentstate", "patrol");
        MoveForward(logicHandler);
        while (true)
        {
            if (!GroundCheck(logicHandler))
            {
                yield return null;
                continue;
            }
            
            if ((!GroundAheadCheck(logicHandler) || WallCheck(logicHandler))
                && Time.frameCount % FrameCountToCheckForGround == 0)
            {
                GetBlackboard(logicHandler).Set<Vector2>("previousmoveinput", GetBlackboard(logicHandler).Get<Vector2>("moveinput") );
                GetBlackboard(logicHandler).Set<Vector2>("moveinput", Vector2.zero);
                yield return TurnAround(logicHandler);
                MoveForward(logicHandler);
            }
            yield return null;
        }
    }
    bool CheckXDistance(CharacterLogicHandler logicHandler)
    {
        GameObject target = GetBlackboard(logicHandler).Get<GameObject>("target");
        if (target == null)
            return false;

        float distanceToTarget = Mathf.Abs(target.transform.position.x - logicHandler.transform.position.x);
        return distanceToTarget <= stopDistance;
    }
    IEnumerator ChaseState(CharacterLogicHandler logicHandler)
    {
        GetBlackboard(logicHandler).Set<string>("currentstate", "chase");
        while (PlayerSightCheck(logicHandler) && PlayerHeightCheck(logicHandler) && PlayerDistanceCheck(logicHandler))
        {
            MoveTowardsTarget(logicHandler);
            while (CheckXDistance(logicHandler))
            {
                GetBlackboard(logicHandler).Set<Vector2>("moveinput", Vector2.zero);
                CombatSystem.CombatManager.instance.AttackFlash(logicHandler.GetComponent<SpriteRenderer>());
                yield return new WaitForSeconds(attackPause);
                yield return Attack(logicHandler);
            }
            yield return null;
        }
        StartPatrol(logicHandler);
    }
    IEnumerator Attack(CharacterLogicHandler logicHandler)
    {
        GameObject target = GetBlackboard(logicHandler).Get<GameObject>("target");
        Vector2 directionToTarget = (target.transform.position - logicHandler.transform.position).normalized;
        logicHandler.GetComponent<CharacterActionHandler>().GetSign(directionToTarget);
        GetBlackboard(logicHandler).Set<bool>("attackpressed", true);
        yield return new WaitForSeconds(attackDuration);
    }
    IEnumerator TurnAround(CharacterLogicHandler logicHandler)
    {
        yield return new WaitForSeconds(turnPauseDuration);
        GetBlackboard(logicHandler).Set<int>("facedirection", -GetBlackboard(logicHandler).Get<int>("facedirection"));
    }
    public void MoveTowardsTarget(CharacterLogicHandler logicHandler)
    {
        GameObject target = GetBlackboard(logicHandler).Get<GameObject>("target");
        if (target == null)
            return;

        Vector2 directionToTarget = (target.transform.position - logicHandler.transform.position).normalized;
        GetBlackboard(logicHandler).Set<Vector2>("moveinput", new Vector2(directionToTarget.x, 0));
        GetBlackboard(logicHandler).Set<int>("facedirection", directionToTarget.x > 0 ? 1 : -1);

    }
    public void StartPatrol(CharacterLogicHandler logicHandler)
    {
        logicHandler.StopAllCoroutines();
        logicHandler.StartCoroutine(PatrolState(logicHandler));
    }
    public void StartChase(CharacterLogicHandler logicHandler)
    {
        logicHandler.StopAllCoroutines();
        logicHandler.StartCoroutine(ChaseState(logicHandler));
    }
    public void MoveForward(CharacterLogicHandler logicHandler)
    {
        int currentDirection = GetBlackboard(logicHandler).Get<int>("facedirection") > 0 ? 1 : -1 ;
        GetBlackboard(logicHandler).Set<Vector2>("moveinput", Vector2.right * currentDirection);
    }

    public bool GroundAheadCheck(CharacterLogicHandler logicHandler)
    {
        return AILogicFunctions.Instance.GroundAheadCheck(logicHandler.transform.position, Vector2.up/2, GetBlackboard(logicHandler).Get<int>("facedirection"), GroundCheckDistance, GroundCheckDistanceFromBody, GroundMask);
    }
    public bool WallCheck(CharacterLogicHandler logicHandler)
    {
        return AILogicFunctions.Instance.WallCheck(logicHandler.transform.position, Vector2.up ,GetBlackboard(logicHandler).Get<int>("facedirection"), GroundCheckDistance, GroundMask);
    }
    public bool GroundCheck(CharacterLogicHandler logicHandler)
    {
        return AILogicFunctions.Instance.GroundCheck(logicHandler.transform.position, Vector2.up / 2, new Vector2(0.5f, 1f), GroundMask);
    }
    public bool PlayerForwardDetection(CharacterLogicHandler logicHandler)
    {
        Collider2D playerHit = AILogicFunctions.Instance.BoxCheckAnchored((Vector2)logicHandler.transform.position + Vector2.up, GetBlackboard(logicHandler).Get<int>("facedirection"), PlayerForwardDetectionDistance, 3f, PlayerMask);
        return playerHit != null;
    }
    public bool PlayerSightCheck(CharacterLogicHandler logicHandler)
    {
        GameObject target = GetBlackboard(logicHandler).Get<GameObject>("target");
        if (target == null)
            return false;

        return !AILogicFunctions.Instance.LineCheck((Vector2)logicHandler.transform.position + Vector2.up, (Vector2)target.transform.position + Vector2.up, 0.5f, GroundMask);
    }
    public bool PlayerDistanceCheck(CharacterLogicHandler logicHandler)
    {
        return Vector2.Distance(logicHandler.transform.position, GetBlackboard(logicHandler).Get<GameObject>("target").transform.position) <= PlayerMaxChaseDistance;
    }
    public bool PlayerHeightCheck(CharacterLogicHandler logicHandler)
    {
        GameObject target = GetBlackboard(logicHandler).Get<GameObject>("target");
        if (target == null)
            return false;

        float heightDifference = Mathf.Abs(target.transform.position.y - logicHandler.transform.position.y);
        return heightDifference <= playerHeightCheck;
    }
    public void FindTarget(CharacterLogicHandler logicHandler)
    {
        if (GetBlackboard(logicHandler).Get<GameObject>("target") != null)
            return;

        Collider2D playerHit = AILogicFunctions.Instance.BoxCheck((Vector2)logicHandler.transform.position + Vector2.up,  new Vector2(PlayerDetectionDistance, PlayerDetectionDistance), PlayerMask);
        if (playerHit != null) 
        GetBlackboard(logicHandler).Set<GameObject>("target", playerHit.gameObject);
    }
}
