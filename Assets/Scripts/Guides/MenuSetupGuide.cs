/*
 * MENU SYSTEM SETUP GUIDE
 * =======================
 * 
 * This guide will help you set up the complete menu system with start menu,
 * game over menu, and cursor control for your YOG game.
 * 
 * STEP 1: QUICK SETUP (RECOMMENDED)
 * ---------------------------------
 * 1. Create an empty GameObject in your scene called "MenuManager"
 * 2. Add the MenuUISetup script to it
 * 3. Click "Setup Menu UI" in the inspector (or right-click component)
 * 4. This will automatically create all menus and UI elements
 * 5. Done! Your menu system is ready
 * 
 * STEP 2: MANUAL SETUP (ADVANCED)
 * -------------------------------
 * If you prefer to set up menus manually:
 * 
 * 1. Create a Canvas in your scene
 * 2. Add the MenuManager script to the Canvas
 * 3. Create UI panels for each menu:
 *    - Start Menu Panel
 *    - Game UI Panel (your existing game UI)
 *    - Pause Menu Panel
 *    - Game Over Panel
 *    - Level Complete Panel
 * 4. Create buttons and text for each panel
 * 5. Assign all UI elements to the MenuManager script
 * 
 * STEP 3: CURSOR CONTROL
 * ----------------------
 * The MenuManager automatically handles cursor visibility:
 * - Cursor is HIDDEN during gameplay (for aiming)
 * - Cursor is VISIBLE in menus
 * - You can customize this in the MenuManager inspector
 * 
 * STEP 4: INTEGRATION
 * ------------------
 * The menu system integrates with your existing systems:
 * - LevelManager: Handles game state transitions
 * - KillCounter: Shows final scores
 * - EnemySpawner: Starts/stops enemy spawning
 * - GameUI: Your existing game UI
 * 
 * STEP 5: CUSTOMIZATION
 * --------------------
 * 
 * Menu Appearance:
 * - Modify colors in MenuUISetup script
 * - Change fonts and sizes
 * - Add your own graphics and backgrounds
 * 
 * Button Functions:
 * - Start Game: Begins gameplay
 * - Restart: Restarts current level
 * - Main Menu: Returns to start menu
 * - Quit: Exits the game
 * - Resume: Continues paused game
 * 
 * Audio:
 * - Assign button click sounds
 * - Add menu music and game music
 * - Configure audio sources
 * 
 * STEP 6: TESTING
 * --------------
 * 1. Play the scene
 * 2. You should see the start menu
 * 3. Click "Start Game" to begin
 * 4. Press Escape to pause
 * 5. Complete the level to see level complete menu
 * 6. Test all buttons and transitions
 * 
 * MENU FLOW:
 * ----------
 * 
 * Start Menu → Start Game → Playing
 * Playing → Pause (Escape) → Resume
 * Playing → Level Complete → Next Level/Main Menu
 * Playing → Game Over → Restart/Main Menu
 * Any Menu → Main Menu → Start Menu
 * 
 * CURSOR BEHAVIOR:
 * ---------------
 * 
 * Start Menu: Cursor visible, unlocked
 * Playing: Cursor hidden, locked (for aiming)
 * Paused: Cursor visible, unlocked
 * Game Over: Cursor visible, unlocked
 * Level Complete: Cursor visible, unlocked
 * 
 * CUSTOMIZATION OPTIONS:
 * ---------------------
 * 
 * MenuManager Settings:
 * - hideCursorInGame: Hide cursor during gameplay
 * - showCursorInMenus: Show cursor in menus
 * - pauseKey: Key to pause game (default: Escape)
 * 
 * MenuUISetup Settings:
 * - gameTitle: Title text for start menu
 * - buttonColor: Color of all buttons
 * - textColor: Color of all text
 * - buttonFont: Font for buttons
 * - textFont: Font for text
 * 
 * ADVANCED FEATURES:
 * -----------------
 * 
 * Settings Menu:
 * - Add a settings button to start menu
 * - Create volume sliders, graphics options, etc.
 * - Save settings to PlayerPrefs
 * 
 * Level Progression:
 * - Implement multiple levels
 * - Add level selection menu
 * - Track progress and unlocks
 * 
 * High Scores:
 * - Save best times and scores
 * - Display leaderboards
 * - Add achievements
 * 
 * TROUBLESHOOTING:
 * ---------------
 * 
 * Menus not showing?
 * - Check if MenuManager is in scene
 * - Verify UI panels are assigned
 * - Check console for errors
 * 
 * Cursor not hiding?
 * - Check hideCursorInGame setting
 * - Verify game state is "Playing"
 * - Check if other scripts are overriding cursor
 * 
 * Buttons not working?
 * - Check if button listeners are assigned
 * - Verify MenuManager is active
 * - Check console for errors
 * 
 * Game not starting?
 * - Check if LevelManager is in scene
 * - Verify EnemySpawner is assigned
 * - Check if game state is correct
 * 
 * PERFORMANCE TIPS:
 * ----------------
 * - Use object pooling for UI elements
 * - Disable unused UI panels
 * - Optimize UI textures and fonts
 * - Use Canvas Groups for complex UI
 */

public class MenuSetupGuide
{
    // This class is just for documentation purposes
    // The actual setup guide is in the comments above
}
