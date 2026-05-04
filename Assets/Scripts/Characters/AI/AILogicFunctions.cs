using UnityEngine;
using UnityEngine.Playables;

namespace AILogicGroup
{
    public class AILogicFunctions : MonoBehaviour
    {
        public static AILogicFunctions Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public bool WallCheck(Vector2 origin, Vector2 offset, int direction,float distance,LayerMask wallMask)
        {
            origin = origin + offset;
            Vector2 rayDir =Vector2.right * direction;
            RaycastHit2D hit =   Physics2D.Raycast(origin, rayDir,distance,wallMask);
            //if (Time.frameCount % 12 == 0)
            //    Debug.DrawRay( origin, rayDir * distance, Color.red, 0.05f);
            return hit.collider != null;
        }
        public bool GroundAheadCheck(Vector2 origin, Vector2 offset, int direction,float raydistance, float distanceFromBody, LayerMask groundMask)
        {
            origin = origin + offset ;
            Vector2 rayDir = (Vector2.down) .normalized;
            Vector2 rayPos = new Vector2(origin.x + (direction * distanceFromBody), origin.y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, rayDir, raydistance, groundMask);
            //if (Time.frameCount % 12 == 0)
            //{
            //    Debug.DrawRay(rayPos, rayDir * raydistance, Color.blue, 0.1f);
            //    Debug.DrawRay(origin, (rayPos - origin ).normalized * distanceFromBody, Color.blue, 0.05f);
            //}
            return hit.collider != null;
        }
        public bool GroundCheck(Vector2 origin, Vector2 offset, Vector2 boxSize, LayerMask groundMask)
        {
            origin = origin + offset;
            Collider2D hit = Physics2D.OverlapBox(origin, boxSize, 0f, groundMask);

            //if(Time.frameCount % 12 == 0)
            //{
            //    Debug.DrawRay(origin, Vector2.down * boxSize.y / 2, Color.green, 0.1f);
            //    Debug.DrawRay(origin, Vector2.right * boxSize.x / 2, Color.green, 0.1f);
            //    Debug.DrawRay(origin, Vector2.left * boxSize.x / 2, Color.green, 0.1f);
            //}
            return hit != null;
        }
        public Collider2D BoxCheck(Vector2 origion, Vector2 size, LayerMask detectionMask)
        {
            Collider2D hit = Physics2D.OverlapBox(origion, size, 0f, detectionMask);
            if (Time.frameCount % 12 == 0)
            {
                DrawBox(origion, size, Vector2.right);
            }
            return hit;

        }
        public Collider2D BoxCheckAnchored( Vector2 anchorPoint, int direction, float width, float height, LayerMask detectionMask)
        {
            Vector2 boxCenter = anchorPoint + Vector2.right * direction * width / 2;
            Vector2 boxSize = new Vector2(width, height);
            Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, detectionMask);

            if (Time.frameCount % 12 == 0)
            {
                //draw box
                DrawBox(boxCenter, boxSize, Vector2.right * direction);

            }
            return hit;
        }

        public bool LineCheck(Vector2 origin, Vector2 targetPos, float lineWidth, LayerMask obstacleMask)
        {
            Vector2 direction = (targetPos - origin).normalized;
            float distance =  Vector2.Distance(origin, targetPos);
            Vector2 boxSize = new Vector2(distance, lineWidth);
            Vector2 boxCenter =  origin + direction * distance * 0.5f;
            bool hit = Physics2D.BoxCast( boxCenter, boxSize, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector2.zero,0f,obstacleMask);

            DrawBox(boxCenter, boxSize, direction);

            return hit;
        }
        public bool CircleCheck( Vector2 origin, float radius, LayerMask targetMask)
        {
            Collider2D hit = Physics2D.OverlapCircle( origin, radius,  targetMask);
            return hit != null;
        }
        private void DrawBox( Vector2 center, Vector2 size, Vector2 direction)
        {
            return;
            Vector2 right =
                direction;

            Vector2 up =
                Vector2.Perpendicular(direction);


            Vector2 halfRight =
                right * size.x * 0.5f;

            Vector2 halfUp =
                up * size.y * 0.5f;


            Vector2 tl =
                center - halfRight + halfUp;

            Vector2 tr =
                center + halfRight + halfUp;

            Vector2 bl =
                center - halfRight - halfUp;

            Vector2 br =
                center + halfRight - halfUp;


            Debug.DrawLine(tl, tr, Color.green,0.1f);
            Debug.DrawLine(tr, br, Color.green, 0.1f);
            Debug.DrawLine(br, bl, Color.green, 0.1f);
            Debug.DrawLine(bl, tl, Color.green, 0.1f);
        }
    }
}