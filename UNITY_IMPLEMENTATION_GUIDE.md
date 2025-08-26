# Unity Match 3 Implementation Guide

## 1. SCENE SETUP

### Step 1: Create the Scene Hierarchy
```
SampleScene
├── Main Camera (position: 3.5, -3.5, -10)
├── GameManagers (Empty GameObject)
│   ├── GameManager
│   ├── Match3Manager
│   ├── AudioManager
│   ├── EffectsManager
│   ├── InputManager
│   └── UIManager
├── GridParent (Empty GameObject - position: 0, 0, 0)
├── Canvas (UI)
│   ├── ScoreText
│   ├── MovesText
│   ├── LevelText
│   ├── PauseButton
│   └── GameOverPanel
└── EventSystem

```

## 2. PREFAB CREATION

### Create Tile Prefab:
1. Create Empty GameObject named "TilePrefab"
2. Add SpriteRenderer component
3. Add BoxCollider2D component
4. Add Tile.cs script
5. Set Collider2D as Trigger = false
6. Save as prefab in Assets/Prefabs/

### Tile Prefab Inspector Settings:
- Transform: Scale (1, 1, 1)
- Sprite Renderer: 
  - Sprite: (assign default tile sprite)
  - Material: Default-Material
  - Color: White
  - Sorting Layer: Default
  - Order in Layer: 0
- Box Collider 2D:
  - Size: (1, 1)
  - Offset: (0, 0)
- Tile Script:
  - Tile Type: 0
  - Move Speed: 10

## 3. SCRIPTABLE OBJECT SETUP

### Create GridSettings:
1. Right-click in Assets folder
2. Create → Match3 → Grid Settings
3. Name it "LevelGridSettings"
4. Configure in Inspector:
   - Rows: 8
   - Columns: 8
   - Tile Type Count: 5
   - Tile Spacing: 1
   - Move Speed: 10
   - Fall Speed: 8
   - Swap Speed: 6

## 4. SPRITE PREPARATION

Create 5 different colored tile sprites:
- Red tile
- Blue tile  
- Green tile
- Yellow tile
- Purple tile

Save them as:
- Assets/Sprites/tile_red.png
- Assets/Sprites/tile_blue.png
- Assets/Sprites/tile_green.png
- Assets/Sprites/tile_yellow.png
- Assets/Sprites/tile_purple.png

Import Settings for each sprite:
- Texture Type: Sprite (2D and UI)
- Pixels Per Unit: 100
- Filter Mode: Point (no filter)
- Compression: None

## 5. MANAGER COMPONENT SETUP

### GameManager Setup:
```
GameObject: GameManager
Components:
- GameManager.cs
  - Current State: Playing
  - Target Score: 1000
  - Moves Limit: 30
  - Time Limit: 300
  - Match3 Manager: (drag Match3Manager)
  - UI Manager: (drag UIManager)
  - Audio Manager: (drag AudioManager)
```

### Match3Manager Setup:
```
GameObject: Match3Manager
Components:
- Match3Manager.cs
  - Use Scriptable Settings: ✓
  - Grid Settings: (drag LevelGridSettings)
  - Tile Prefab: (drag TilePrefab)
  - Grid Parent: (drag GridParent)
  - Rows: 8
  - Columns: 8
  - Tile Type Count: 5
  - Tile Sprites: (assign 5 tile sprites)
  - Tile Spacing: 1
  - Fall Speed: 10
  - Swap Speed: 8
```

### AudioManager Setup:
```
GameObject: AudioManager
Components:
- AudioManager.cs
- AudioSource (for music)
  - Loop: ✓
  - Volume: 0.7
- AudioSource (for SFX)
  - Loop: ✗
  - Volume: 1.0
```

### UIManager Setup:
```
GameObject: UIManager
Components:
- UIManager.cs
  - Score Text: (drag ScoreText UI element)
  - Moves Text: (drag MovesText UI element)  
  - Level Text: (drag LevelText UI element)
  - Moves Remaining: 30
  - Current Level: 1
```

## 6. UI CANVAS SETUP

### Create UI Elements:
1. Canvas (Screen Space - Overlay)
2. ScoreText (Text component):
   - Position: Top Left
   - Text: "Score: 0"
   - Font Size: 24
   - Color: White

3. MovesText (Text component):
   - Position: Top Right  
   - Text: "Moves: 30"
   - Font Size: 24
   - Color: White

4. LevelText (Text component):
   - Position: Top Center
   - Text: "Level: 1"
   - Font Size: 24
   - Color: White

## 7. CAMERA CONFIGURATION

Main Camera Settings:
- Position: (3.5, -3.5, -10)
- Rotation: (0, 0, 0)
- Projection: Orthographic
- Size: 6
- Clipping Planes: Near 0.3, Far 1000
- Clear Flags: Solid Color
- Background: Dark Blue or Black

## 8. PHYSICS SETTINGS

### 2D Physics Settings (Edit → Project Settings → Physics 2D):
- Gravity: (0, 0) - We handle gravity manually
- Default Material: None

### Layer Setup (optional but recommended):
- Layer 8: Tiles
- Layer 9: UI
- Layer 10: Effects

## 9. INPUT SYSTEM

### If using Old Input System:
- No additional setup needed

### If using New Input System:
1. Install Input System package
2. Create Input Actions asset
3. Add Touch and Mouse actions

## 10. AUDIO RESOURCES

Create audio clips for:
- Background music (Assets/Audio/bgm_game.mp3)
- Swap sound (Assets/Audio/sfx_swap.wav)
- Match sound (Assets/Audio/sfx_match.wav)
- Invalid move (Assets/Audio/sfx_invalid.wav)
- Combo sound (Assets/Audio/sfx_combo.wav)
- Game over (Assets/Audio/sfx_gameover.wav)

## 11. PARTICLE EFFECTS (Optional)

Create simple particle systems:
1. Match Effect: Small explosion of colored particles
2. Combo Effect: Larger burst with sparkles
3. Tile Destroy: Small puff of smoke

## 12. BUILD SETTINGS

### For Mobile (Android/iOS):
- Orientation: Portrait or Landscape
- Target SDK: Latest
- Scripting Backend: IL2CPP
- Target Architecture: ARM64

### For WebGL:
- Compression Format: Gzip
- WebGL Template: Default or Custom

## 13. TESTING CHECKLIST

✅ Camera shows 8x8 grid properly
✅ Tiles are clickable and highlight on selection  
✅ Adjacent tiles can be swapped
✅ Matches of 3+ tiles are detected and removed
✅ Tiles fall down when others are removed
✅ New tiles spawn from top
✅ Score increases with matches
✅ UI updates correctly
✅ Audio plays for different events
✅ Game over conditions work
✅ Performance is smooth (60 FPS)

## 14. COMMON ISSUES & SOLUTIONS

### Issue: Tiles not clickable
**Solution:** 
- Ensure tiles have Collider2D components
- Check camera raycast settings
- Verify Input Manager is active

### Issue: Grid positioning wrong
**Solution:**
- Check GridParent position (should be 0,0,0)
- Verify tileSpacing in Match3Manager
- Adjust camera position

### Issue: Sprites not showing
**Solution:**
- Assign sprites to GridSettings ScriptableObject
- Check sprite import settings
- Verify SpriteRenderer on tile prefab

### Issue: Performance problems
**Solution:**
- Use object pooling for tiles
- Limit particle effects
- Optimize sprite sizes

## 15. NEXT STEPS

After basic implementation works:
1. Add juice (screen shake, particles, animations)
2. Implement special tiles (bombs, striped tiles)
3. Add level progression
4. Create main menu and settings
5. Add tutorial system
6. Implement save/load system
7. Add achievements and leaderboards

## 16. FOLDER STRUCTURE

```
Assets/
├── Scenes/
│   └── GameScene.unity
├── Scripts/
│   ├── Managers/
│   │   ├── Match3Manager.cs
│   │   ├── GameManager.cs
│   │   ├── UIManager.cs
│   │   ├── AudioManager.cs
│   │   ├── EffectsManager.cs
│   │   ├── InputManager.cs
│   │   └── Tile.cs
│   └── Utilities/
│       └── GridSettings.cs
├── Prefabs/
│   ├── TilePrefab.prefab
│   └── Managers.prefab
├── Sprites/
│   ├── tile_red.png
│   ├── tile_blue.png
│   ├── tile_green.png
│   ├── tile_yellow.png
│   └── tile_purple.png
├── Audio/
│   ├── bgm_game.mp3
│   ├── sfx_swap.wav
│   ├── sfx_match.wav
│   └── sfx_combo.wav
├── ScriptableObjects/
│   └── LevelGridSettings.asset
└── Materials/
    └── TileMaterial.mat
```
