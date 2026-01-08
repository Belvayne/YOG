using System.Reflection;
using UnityEngine;

public class TakodachiLogic : EnemyController
{
    // Health (mirrors fields in base for inspector tuning)
    [SerializeField] private float maxHealth =100f;
    [SerializeField] private float currentHealth =100f;

    // Flying/hovering
    [SerializeField] private float maxHeight =5f; // maximum distance allowed above the nearest surface below
    [SerializeField] private float currentHeight =0f; // updated at runtime

    // Movement
    [SerializeField] private float speed =5f;

    // Obstacle detection and avoidance
    [SerializeField] private float detectionDistance =3f; // how far ahead to check for obstacles
    [SerializeField] private float detectionSearchHeight =30f; // how high above to start the downward ray
    [SerializeField] private float clearanceAboveTop =0.5f; // how much clearance above obstacle top is required to pass
    [SerializeField] private float sideAvoidDistance =3f; // how far to move sideways when going around
    [SerializeField] private float avoidanceDuration =1.0f; // how long to keep steering around

    private float avoidUntilTime =0f;
    private Vector3 avoidanceTarget = Vector3.zero;

    // Attack tuning (mirror of base's attack cooldown)
    [SerializeField] private float attackSpeed =1.5f; // corresponds to base's attackCooldown
    [SerializeField] private float lastAttackTime = -Mathf.Infinity; // mirror of timing

    // Additional tuning mirrors
    [SerializeField] private float attackRange =2f;
    [SerializeField] private int attackDamage =10;
    [SerializeField] private float hitForceMagnitude =5f;
    [SerializeField] private float rotationSpeed =5f;

    private Transform playerTransform;

    private void Start()
    {
        // Initialize player transform here and mirror it into base via reflection
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Remove NavMeshAgent if present: flying enemy uses manual movement
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            Destroy(agent);

        // Sync serialized tuning values into private fields of the base EnemyController using reflection
        var baseType = typeof(EnemyController);

        // currentHealth
        var baseCurrentField = baseType.GetField("currentHealth", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseCurrentField != null)
            baseCurrentField.SetValue(this, currentHealth);

        // maxHealth
        var baseMaxField = baseType.GetField("maxHealth", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseMaxField != null)
            baseMaxField.SetValue(this, maxHealth);

        // moveSpeed -> speed
        var baseMoveSpeedField = baseType.GetField("moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseMoveSpeedField != null)
            baseMoveSpeedField.SetValue(this, speed);

        // attackCooldown -> attackSpeed
        var baseAttackCooldownField = baseType.GetField("attackCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseAttackCooldownField != null)
            baseAttackCooldownField.SetValue(this, attackSpeed);

        // lastAttackTime (mirror)
        var baseLastAttackField = baseType.GetField("lastAttackTime", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseLastAttackField != null)
            baseLastAttackField.SetValue(this, lastAttackTime);

        // playerTransform
        var basePlayerTransformField = baseType.GetField("playerTransform", BindingFlags.Instance | BindingFlags.NonPublic);
        if (basePlayerTransformField != null)
            basePlayerTransformField.SetValue(this, playerTransform);

        // rotationSpeed
        var baseRotationField = baseType.GetField("rotationSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseRotationField != null)
            baseRotationField.SetValue(this, rotationSpeed);

        // attackRange
        var baseAttackRangeField = baseType.GetField("attackRange", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseAttackRangeField != null)
            baseAttackRangeField.SetValue(this, attackRange);

        // attackDamage
        var baseAttackDamageField = baseType.GetField("attackDamage", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseAttackDamageField != null)
            baseAttackDamageField.SetValue(this, attackDamage);

        // hitForceMagnitude
        var baseHitForceField = baseType.GetField("hitForceMagnitude", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseHitForceField != null)
            baseHitForceField.SetValue(this, hitForceMagnitude);
    }

    private void Update()
    {
        // Avoid running if base reports dead or no player
        if (IsDead())
            return;

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            // mirror into base if possible
            var basePlayerTransformField = typeof(EnemyController).GetField("playerTransform", BindingFlags.Instance | BindingFlags.NonPublic);
            if (basePlayerTransformField != null)
                basePlayerTransformField.SetValue(this, playerTransform);
        }

        if (playerTransform == null)
            return;

        // compute horizontal forward direction toward player (XZ only)
        Vector3 toPlayer = playerTransform.position - transform.position;
        Vector3 forwardXZ = new Vector3(toPlayer.x,0f, toPlayer.z);
        if (forwardXZ.sqrMagnitude <0.0001f)
            forwardXZ = transform.forward;
        forwardXZ.Normalize();

        Vector3 horizontalTarget = transform.position;

        // If we're currently avoiding, keep steering to the avoidance target until timeout
        if (Time.time < avoidUntilTime)
        {
            horizontalTarget = new Vector3(avoidanceTarget.x, transform.position.y, avoidanceTarget.z);
        }
        else
        {
            // Look ahead at a point in the forward direction and detect the top surface there
            Vector3 aheadPoint = transform.position + forwardXZ * detectionDistance;
            float aheadTopY;
            bool hasAheadTop = TryGetTopYAt(aheadPoint, out aheadTopY);

            // Get ground underneath current position
            float currentGroundY;
            bool hasCurrentGround = TryGetTopYAt(new Vector3(transform.position.x, transform.position.y, transform.position.z), out currentGroundY);
            if (!hasCurrentGround)
                currentGroundY = transform.position.y - currentHeight; // fallback

            bool needAvoid = false;

            if (hasAheadTop)
            {
                // compute required Y to clear obstacle top
                float requiredYToClear = aheadTopY + clearanceAboveTop;
                // compute how high that required Y is above current ground
                float requiredAboveCurrentGround = requiredYToClear - currentGroundY;

                if (requiredAboveCurrentGround <= maxHeight)
                {
                    // We can clear by ascending: set horizontal target toward forward (we will adjust Y below)
                    horizontalTarget = new Vector3(aheadPoint.x, transform.position.y, aheadPoint.z);
                }
                else
                {
                    // Obstacle too tall to clear while respecting maxHeight -> need to avoid laterally
                    needAvoid = true;
                }
            }
            else
            {
                // No surface ahead detected: just head to player horizontally
                horizontalTarget = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            }

            if (needAvoid)
            {
                // test left and right to find side that is clearer
                Vector3 leftDir = Quaternion.Euler(0f, -90f,0f) * forwardXZ;
                Vector3 rightDir = Quaternion.Euler(0f,90f,0f) * forwardXZ;

                Vector3 leftCheck = transform.position + leftDir * sideAvoidDistance + forwardXZ * detectionDistance;
                Vector3 rightCheck = transform.position + rightDir * sideAvoidDistance + forwardXZ * detectionDistance;

                float leftTop = float.MaxValue;
                float rightTop = float.MaxValue;
                bool leftHas = TryGetTopYAt(leftCheck, out leftTop);
                bool rightHas = TryGetTopYAt(rightCheck, out rightTop);

                bool leftClear = leftHas && ((leftTop + clearanceAboveTop - currentGroundY) <= maxHeight);
                bool rightClear = rightHas && ((rightTop + clearanceAboveTop - currentGroundY) <= maxHeight);

                Vector3 chosenSide;
                if (leftClear && !rightClear)
                    chosenSide = leftCheck;
                else if (rightClear && !leftClear)
                    chosenSide = rightCheck;
                else if (leftClear && rightClear)
                    chosenSide = (leftTop < rightTop) ? leftCheck : rightCheck;
                else
                    chosenSide = (leftTop < rightTop) ? leftCheck : rightCheck; // both blocked: pick lower top and try to circumvent

                // set avoidance target and timer
                avoidanceTarget = new Vector3(chosenSide.x, transform.position.y, chosenSide.z);
                avoidUntilTime = Time.time + avoidanceDuration;
                horizontalTarget = avoidanceTarget;
            }
        }

        // Move horizontally toward the computed target
        Vector3 newPos = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);

        // Vertical control: maintain hovering within maxHeight over the nearest surface beneath us
        RaycastHit hit;
        float maxSearchDistance =100f;
        if (Physics.Raycast(transform.position + Vector3.up *0.1f, Vector3.down, out hit, maxSearchDistance))
        {
            currentHeight = hit.distance -0.1f; // subtract the small offset used in the ray origin

            // We want to remain within maxHeight above the surface beneath us.
            float desiredY = transform.position.y;
            float allowedY = hit.point.y + maxHeight;

            // If avoidance-based ascent was requested (to clear an obstacle directly ahead), we may need to ascend above current Y
            // Check immediate forward point again to see if we should clear an object by ascending
            Vector3 aheadPoint2 = transform.position + new Vector3((playerTransform.position - transform.position).x,0f, (playerTransform.position - transform.position).z).normalized * detectionDistance;
            float aheadTopY2;
            if (TryGetTopYAt(aheadPoint2, out aheadTopY2))
            {
                float requiredYToClear = aheadTopY2 + clearanceAboveTop;
                // Only ascend if required Y is not higher than allowed by nearest surface beneath (so we still respect maxHeight rule)
                if (requiredYToClear <= (hit.point.y + maxHeight))
                {
                    desiredY = Mathf.Lerp(transform.position.y, requiredYToClear, Time.deltaTime * speed);
                }
            }

            // Clamp downward/upward to stay within allowed bounds
            if (desiredY > allowedY)
                desiredY = Mathf.Lerp(transform.position.y, allowedY, Time.deltaTime * speed);
            else if (desiredY < hit.point.y +0.5f)
                desiredY = Mathf.Lerp(transform.position.y, hit.point.y +0.5f, Time.deltaTime * speed);

            newPos.y = desiredY;
        }
        else
        {
            // No surface found below within search distance: keep current Y
            currentHeight = float.PositiveInfinity;
        }

        transform.position = newPos;

        // Face the player (yaw only)
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y =0f;
        if (lookDir.sqrMagnitude >0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        // Attack-only logic: deal damage when within range and cooldown passed
        float sqrDistance = (playerTransform.position - transform.position).sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;
        if (sqrDistance <= attackRangeSqr && Time.time >= lastAttackTime + attackSpeed)
        {
            var playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null && playerController.GetCurrentHealth() >0)
            {
                // Determine closest point on the player's collider (fallback to player position)
                Vector3 hitPoint = playerTransform.position;
                Collider playerCollider = playerTransform.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    hitPoint = playerCollider.ClosestPoint(transform.position);
                }
                else
                {
                    var cc = playerTransform.GetComponent<CharacterController>();
                    if (cc != null)
                        hitPoint = cc.transform.position;
                }

                Vector3 hitDirection = (hitPoint - transform.position);
                if (hitDirection.sqrMagnitude >0.0001f)
                    hitDirection.Normalize();
                Vector3 hitForce = hitDirection * hitForceMagnitude;

                playerController.TakeDamage(hitPoint, hitForce, attackDamage);
            }

            lastAttackTime = Time.time;

            // mirror lastAttackTime into base field as well
            var baseLastAttackField = typeof(EnemyController).GetField("lastAttackTime", BindingFlags.Instance | BindingFlags.NonPublic);
            if (baseLastAttackField != null)
                baseLastAttackField.SetValue(this, lastAttackTime);
        }

        // Mirror health and timing values back from base to show correct values in this inspector
        var baseType = typeof(EnemyController);
        var baseCurrentField = baseType.GetField("currentHealth", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseCurrentField != null)
        {
            var val = baseCurrentField.GetValue(this);
            if (val is float cf)
                currentHealth = cf;
        }
        var baseMaxField = baseType.GetField("maxHealth", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseMaxField != null)
        {
            var val = baseMaxField.GetValue(this);
            if (val is float mf)
                maxHealth = mf;
        }

        var baseLastAttack = baseType.GetField("lastAttackTime", BindingFlags.Instance | BindingFlags.NonPublic);
        if (baseLastAttack != null)
        {
            var val = baseLastAttack.GetValue(this);
            if (val is float f)
                lastAttackTime = f;
        }
    }

    // Helper: cast from high above `pos` straight down and return the y coordinate of the first hit (top surface at that horizontal location)
    private bool TryGetTopYAt(Vector3 pos, out float topY)
    {
        RaycastHit hit;
        Vector3 start = new Vector3(pos.x, pos.y + detectionSearchHeight, pos.z);
        if (Physics.Raycast(start, Vector3.down, out hit, detectionSearchHeight *2f))
        {
            topY = hit.point.y;
            return true;
        }
        topY =0f;
        return false;
    }
}
