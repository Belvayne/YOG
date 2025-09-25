/*
 * ENEMY MOVEMENT & AI SETUP GUIDE
 * ===============================
 * 
 * This guide will help you set up enemy AI that makes enemies walk towards the player.
 * 
 * STEP 1: ENEMY PREFAB SETUP
 * ---------------------------
 * 1. Select your SSRB enemy prefab
 * 2. Add a Rigidbody component (if not already present)
 * 3. Add a Collider component (if not already present)
 * 4. Tag your enemy as "Enemy" (optional but recommended)
 * 5. Make sure your player is tagged as "Player"
 * 
 * STEP 2: ENEMY SPAWNER CONFIGURATION
 * -----------------------------------
 * 1. Select your EnemySpawner in the scene
 * 2. In the Enemy Settings section:
 *    - Enable "Add Movement Script" ✓
 *    - Set "Enemy Move Speed" to 3.0 (or desired speed)
 *    - Set "Enemy Detection Range" to 10.0 (how far they can see)
 *    - Set "Enemy Attack Range" to 2.0 (how close to attack)
 * 3. Choose movement type:
 *    - "Use NavMesh" = false (for simple movement)
 *    - "Use NavMesh" = true (for advanced pathfinding)
 * 
 * STEP 3: NAVMESH SETUP (OPTIONAL)
 * --------------------------------
 * If you want advanced pathfinding:
 * 1. Go to Window > AI > Navigation
 * 2. Select your ground/floor objects
 * 3. Check "Navigation Static" in the Inspector
 * 4. Click "Bake" in the Navigation window
 * 5. Enable "Use NavMesh" in EnemySpawner
 * 
 * STEP 4: PLAYER SETUP
 * --------------------
 * 1. Make sure your player has a "Player" tag
 * 2. Ensure your player has a Health component
 * 3. Position your player in the scene
 * 
 * STEP 5: TESTING
 * ---------------
 * 1. Play the scene
 * 2. Enemies should spawn and start walking towards you
 * 3. They should stop and attack when close enough
 * 4. Check console for debug messages
 * 
 * MOVEMENT TYPES:
 * ---------------
 * 
 * Simple Movement (Recommended for SSRB):
 * - Uses Rigidbody physics
 * - Direct movement towards player
 * - No pathfinding required
 * - Good for simple levels
 * 
 * NavMesh Movement (Advanced):
 * - Uses Unity's NavMesh system
 * - Avoids obstacles automatically
 * - Requires NavMesh baking
 * - Better for complex levels
 * 
 * CUSTOMIZATION OPTIONS:
 * ---------------------
 * 
 * Movement Speed:
 * - Adjust "Enemy Move Speed" in EnemySpawner
 * - Higher values = faster enemies
 * - Lower values = slower enemies
 * 
 * Detection Range:
 * - Adjust "Enemy Detection Range" in EnemySpawner
 * - Higher values = enemies see you from farther away
 * - Lower values = enemies only see you when close
 * 
 * Attack Range:
 * - Adjust "Enemy Attack Range" in EnemySpawner
 * - Higher values = enemies attack from farther away
 * - Lower values = enemies must get closer to attack
 * 
 * Behavior Options:
 * - "Always Chase" = enemies always move towards player
 * - "Can Attack" = enemies can damage player when close
 * - "Can Wander" = enemies wander when not chasing (NavMesh only)
 * 
 * VISUAL FEEDBACK:
 * ---------------
 * 
 * Detection Indicators:
 * - Add a particle system or sprite to "Detection Indicator"
 * - Shows when enemy detects player
 * 
 * Attack Indicators:
 * - Add a particle system or sprite to "Attack Indicator"
 * - Shows when enemy attacks
 * 
 * ANIMATION SETUP:
 * ---------------
 * If your enemy has an Animator:
 * 1. Create animation parameters:
 *    - "IsWalking" (Bool)
 *    - "IsChasing" (Bool)
 *    - "IsAttacking" (Bool)
 *    - "Speed" (Float)
 * 2. The scripts will automatically set these parameters
 * 
 * TROUBLESHOOTING:
 * ---------------
 * 
 * Enemies not moving?
 * - Check if Rigidbody is present
 * - Check if "Add Movement Script" is enabled
 * - Check if player has "Player" tag
 * - Check console for errors
 * 
 * Enemies moving too fast/slow?
 * - Adjust "Enemy Move Speed" in EnemySpawner
 * - Check Rigidbody drag settings
 * 
 * Enemies not detecting player?
 * - Check "Enemy Detection Range" value
 * - Check if player has "Player" tag
 * - Check if player is within range
 * 
 * Enemies not attacking?
 * - Check "Enemy Attack Range" value
 * - Check if "Can Attack" is enabled
 * - Check if player has Health component
 * 
 * PERFORMANCE TIPS:
 * ----------------
 * - Use simple movement for better performance
 * - Limit max enemies to prevent lag
 * - Use LOD system for distant enemies
 * - Optimize enemy prefab (reduce polygon count)
 * - Use object pooling for better performance
 */

public class EnemyMovementSetupGuide
{
    // This class is just for documentation purposes
    // The actual setup guide is in the comments above
}
