/*
 * ENEMY SPAWNER & LEVEL SYSTEM SETUP GUIDE
 * ========================================
 * 
 * This guide will help you set up the complete enemy spawning and level completion system.
 * 
 * STEP 1: ENEMY SPAWNER SETUP
 * ---------------------------
 * 1. Create an empty GameObject in your scene called "EnemySpawner"
 * 2. Add the EnemySpawner script to it
 * 3. Assign your SSRB enemy prefab to the "Enemy Prefab" field
 * 4. Configure spawn settings:
 *    - Spawn Interval: 2.0 (spawns every 2 seconds)
 *    - Max Enemies: 10 (maximum enemies on screen)
 *    - Spawn On Start: true (starts spawning immediately)
 *    - Continuous Spawning: true (keeps spawning until stopped)
 * 
 * STEP 2: SPAWN POINTS SETUP
 * --------------------------
 * Option A - Random Spawn Points:
 * 1. Create empty GameObjects where you want enemies to spawn
 * 2. Add the SpawnPoint script to each one
 * 3. Position them around your level
 * 4. Assign them to the EnemySpawner's "Spawn Points" array
 * 
 * Option B - Spawn Area:
 * 1. On EnemySpawner, enable "Use Spawn Area"
 * 2. Set "Spawn Area Center" to the center of your level
 * 3. Set "Spawn Area Size" to cover your level (e.g., 20x0x20)
 * 4. Set "Ground Layer" to your ground layer for proper positioning
 * 
 * Option C - Spawn Radius:
 * 1. Position the EnemySpawner where you want the center
 * 2. Set "Spawn Radius" to the desired spawn area size
 * 3. Set "Ground Layer" for proper ground positioning
 * 
 * STEP 3: KILL COUNTER SETUP
 * --------------------------
 * 1. Create an empty GameObject called "KillCounter"
 * 2. Add the KillCounter script to it
 * 3. Set "Target Kills" to your desired kill count (e.g., 10)
 * 4. Enable "Reset On Start" to start with 0 kills
 * 5. Enable "Update UI" to automatically update UI elements
 * 
 * STEP 4: LEVEL MANAGER SETUP
 * ---------------------------
 * 1. Create an empty GameObject called "LevelManager"
 * 2. Add the LevelManager script to it
 * 3. Set "Target Kills" to match your KillCounter
 * 4. Configure level settings:
 *    - Level Complete Delay: 2.0 (delay before next level)
 *    - Auto Load Next Level: false (manual level progression)
 *    - Next Level Name: "NextLevel" (name of next scene)
 * 
 * STEP 5: UI SETUP
 * ----------------
 * 1. Create a Canvas in your scene
 * 2. Add UI elements for:
 *    - Kill Count Text (TextMeshPro): "Kills: 0/10"
 *    - Kill Progress Bar (Slider): Shows kill progress
 *    - Level Complete Text (TextMeshPro): "Level Complete!"
 * 3. Assign these to the GameUI script on your player
 * 4. The GameUI will automatically update these elements
 * 
 * STEP 6: ENEMY PREFAB SETUP
 * --------------------------
 * 1. Set up your SSRB enemy prefab with:
 *    - Rigidbody component
 *    - Collider component
 *    - SimpleEnemyHealth script
 *    - CrazyPhysicsController script
 * 2. Make sure the enemy prefab is tagged as "Enemy" (optional)
 * 3. Assign the prefab to the EnemySpawner
 * 
 * STEP 7: TESTING
 * ---------------
 * 1. Play the scene
 * 2. Enemies should spawn every 2 seconds
 * 3. Shoot enemies to see kill counter increase
 * 4. When target kills reached, level should complete
 * 5. Check console for debug messages
 * 
 * ADVANCED FEATURES:
 * -----------------
 * 
 * Wave System:
 * - Enable "Use Waves" on EnemySpawner
 * - Set "Enemies Per Wave" (e.g., 5)
 * - Set "Wave Delay" (e.g., 5 seconds between waves)
 * - Set "Max Waves" (e.g., 10 waves total)
 * 
 * Spawn Point Customization:
 * - Add SpawnPoint script to individual spawn points
 * - Set "Spawn Radius" for random positioning
 * - Enable "Use Ground Detection" for proper ground placement
 * - Set "Ground Layer" to your ground layer
 * 
 * Level Progression:
 * - Set "Auto Load Next Level" to true for automatic progression
 * - Create multiple scenes for different levels
 * - Use LevelManager to handle scene transitions
 * 
 * UI Customization:
 * - Modify kill text format in KillCounter
 * - Add animations to UI elements
 * - Create level complete and game over screens
 * - Add sound effects for kills and level completion
 * 
 * TROUBLESHOOTING:
 * ---------------
 * - No enemies spawning? Check if EnemySpawner is active and prefab is assigned
 * - Enemies not dying? Check if SimpleEnemyHealth is on enemy prefab
 * - Kill counter not updating? Check if KillCounter is in scene and UI is assigned
 * - Level not completing? Check if target kills match between KillCounter and LevelManager
 * - Enemies spawning in wrong places? Check spawn point positions and ground layer
 * 
 * PERFORMANCE TIPS:
 * ----------------
 * - Limit max enemies to prevent performance issues
 * - Use object pooling for better performance
 * - Disable spawn points that are too close to player
 * - Use LOD system for distant enemies
 * - Optimize enemy prefab (reduce polygon count, use efficient materials)
 */

public class EnemySpawnerSetupGuide
{
    // This class is just for documentation purposes
    // The actual setup guide is in the comments above
}
