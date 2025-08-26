# Step-by-Step Unity Setup Instructions

## STEP 1: PREPARE YOUR UNITY PROJECT

### 1.1 Open your Unity project
- Your project should already be open with the existing scene

### 1.2 Create folder structure
```
Right-click in Assets folder → Create Folder:
- Sprites
- Audio  
- Prefabs
- ScriptableObjects
- Materials
```

## STEP 2: CREATE TILE SPRITES

Since you don't have tile sprites yet, let's create simple colored squares:

### 2.1 Create colored tile sprites in Unity:
1. **Create → 2D → Sprites → Square** (repeat 5 times)
2. Name them: tile_red, tile_blue, tile_green, tile_yellow, tile_purple
3. For each sprite:
   - Select in Project window
   - In Inspector, change **Color** to the respective color
   - Move to **Assets/Sprites/** folder

### 2.2 Alternative - Use Unity's built-in sprites:
1. Create 5 materials with different colors
2. Apply to the default Unity cube sprite
3. Or download free tile sprites from Unity Asset Store

## STEP 3: CREATE THE TILE PREFAB

### 3.1 Create tile prefab:
1. **GameObject → Create Empty** (name it "TilePrefab")
2. **Add Component → Rendering → Sprite Renderer**
3. **Add Component → Physics 2D → Box Collider 2D** 
4. **Add Component → Script → Tile** (our Tile.cs script)

### 3.2 Configure tile prefab:
- **Transform:** Position (0,0,0), Scale (1,1,1)
- **Sprite Renderer:** 
  - Sprite: tile_red (default)
  - Material: Sprites-Default
  - Color: White (255,255,255,255)
- **Box Collider 2D:**
  - Size: (1, 1)
  - Is Trigger: FALSE
- **Tile Script:**
  - Tile Type: 0
  - Move Speed: 10

### 3.3 Save as prefab:
1. Drag "TilePrefab" from Hierarchy to **Assets/Prefabs/** folder
2. Delete the TilePrefab from Hierarchy

## STEP 4: CREATE GRID SETTINGS SCRIPTABLE OBJECT

### 4.1 Create the ScriptableObject:
1. **Right-click in Assets/ScriptableObjects/**
2. **Create → Match3 → Grid Settings**
3. Name it "**LevelGridSettings**"

### 4.2 Configure Grid Settings:
- **Rows:** 8
- **Columns:** 8  
- **Tile Type Count:** 5
- **Tile Spacing:** 1
- **Move Speed:** 10
- **Fall Speed:** 8
- **Swap Speed:** 6
- **Base Score:** 10
- **Combo Multiplier:** 1.5

### 4.3 Assign tile sprites:
- **Tile Sprites:** Set size to 5
- Assign your 5 colored tile sprites

## STEP 5: SETUP SCENE HIERARCHY

### 5.1 Create manager GameObjects:
1. **GameObject → Create Empty** (name: "GameManagers")
2. Under GameManagers, create empty GameObjects:
   - GameManager
   - Match3Manager  
   - AudioManager
   - UIManager
   - EffectsManager
   - InputManager

### 5.2 Create grid parent:
1. **GameObject → Create Empty** (name: "GridParent")
2. Position: (0, 0, 0)

### 5.3 Setup camera:
- Select **Main Camera**
- Position: (3.5, -3.5, -10)
- **Projection:** Orthographic
- **Size:** 6

## STEP 6: ADD SCRIPTS TO MANAGERS

### 6.1 Add scripts to each manager:
- **GameManager** → Add Component → Game Manager (script)
- **Match3Manager** → Add Component → Match 3Manager (script)
- **AudioManager** → Add Component → Audio Manager (script)
- **UIManager** → Add Component → UI Manager (script)
- **EffectsManager** → Add Component → Effects Manager (script)
- **InputManager** → Add Component → Input Manager (script)

## STEP 7: CREATE UI CANVAS

### 7.1 Create Canvas:
1. **GameObject → UI → Canvas**
2. **Canvas Scaler:** Scale With Screen Size
3. **Reference Resolution:** 1920x1080

### 7.2 Create UI Text elements:
Under Canvas, create:
1. **UI → Text** (name: "ScoreText")
   - **Rect Transform:** Anchor: Top Left
   - **Text:** "Score: 0"
   - **Font Size:** 36
   - **Color:** White

2. **UI → Text** (name: "MovesText")  
   - **Rect Transform:** Anchor: Top Right
   - **Text:** "Moves: 30"
   - **Font Size:** 36
   - **Color:** White

3. **UI → Text** (name: "LevelText")
   - **Rect Transform:** Anchor: Top Center  
   - **Text:** "Level: 1"
   - **Font Size:** 36
   - **Color:** White

## STEP 8: CONFIGURE MATCH3MANAGER

### 8.1 Select Match3Manager GameObject:
- **Use Scriptable Settings:** ✓ (checked)
- **Grid Settings:** Drag "LevelGridSettings" from Assets
- **Tile Prefab:** Drag "TilePrefab" from Assets/Prefabs
- **Grid Parent:** Drag "GridParent" from Hierarchy
- **Tile Sprites:** Assign your 5 tile sprites (same as in GridSettings)

### 8.2 Configure values:
- **Rows:** 8
- **Columns:** 8
- **Tile Type Count:** 5  
- **Tile Spacing:** 1
- **Fall Speed:** 10
- **Swap Speed:** 8

## STEP 9: CONFIGURE UI MANAGER

### 9.1 Select UIManager GameObject:
- **Score Text:** Drag "ScoreText" from Canvas
- **Moves Text:** Drag "MovesText" from Canvas
- **Level Text:** Drag "LevelText" from Canvas
- **Moves Remaining:** 30
- **Current Level:** 1

## STEP 10: CONFIGURE AUDIO MANAGER

### 10.1 Add AudioSource components:
1. Select **AudioManager**
2. **Add Component → Audio → Audio Source** (for music)
3. **Add Component → Audio → Audio Source** (for SFX)

### 10.2 Configure AudioSources:
- **First AudioSource (Music):**
  - Loop: ✓
  - Volume: 0.7
- **Second AudioSource (SFX):**
  - Loop: ✗  
  - Volume: 1.0

## STEP 11: CONFIGURE GAME MANAGER

### 11.1 Select GameManager GameObject:
- **Target Score:** 1000
- **Moves Limit:** 30
- **Time Limit:** 300
- **Match3 Manager:** Drag Match3Manager
- **UI Manager:** Drag UIManager  
- **Audio Manager:** Drag AudioManager

## STEP 12: TEST THE BASIC SETUP

### 12.1 Press Play:
You should see:
- ✅ 8x8 grid of colored tiles
- ✅ UI showing Score: 0, Moves: 30, Level: 1  
- ✅ Camera positioned to see the full grid

### 12.2 Test interactions:
- ✅ Click tiles (should highlight yellow)
- ✅ Click adjacent tiles (should swap if valid match)
- ✅ Watch tiles fall and new ones spawn
- ✅ Score should increase with matches

## STEP 13: ADD AUDIO (OPTIONAL)

### 13.1 Find free audio clips:
- Search "8-bit game sounds" or "puzzle game audio"
- Download from freesound.org or Unity Asset Store
- Save to Assets/Audio/ folder

### 13.2 Assign audio clips:
- Select **AudioManager**
- Assign clips to respective fields in script

## STEP 14: BUILD AND TEST

### 14.1 Build Settings:
1. **File → Build Settings**
2. **Add Open Scenes**
3. Select platform (PC, Mac, Android, iOS, WebGL)
4. **Build**

## TROUBLESHOOTING COMMON ISSUES

### Issue: Scripts not found
**Solution:** Make sure all .cs files are in Assets/Scripts/ and Unity has compiled them (check Console for errors)

### Issue: Tiles not appearing
**Solution:** 
- Check if GridParent is assigned
- Verify tile prefab has SpriteRenderer
- Check camera position and size

### Issue: Can't click tiles  
**Solution:**
- Ensure tile prefab has Collider2D
- Check if InputManager is active
- Verify camera has Physics2DRaycaster

### Issue: Grid size wrong
**Solution:**
- Check GridSettings values (8x8)
- Verify tileSpacing = 1
- Adjust camera size if needed

### Issue: UI not showing
**Solution:** 
- Check Canvas render mode
- Verify UI elements are assigned to UIManager
- Check EventSystem exists in scene

## NEXT STEPS AFTER BASIC SETUP WORKS

1. **Add juice:** Screen shake, particle effects, smooth animations
2. **Sound effects:** Add audio clips for swaps, matches, combos
3. **Special tiles:** Bombs, striped tiles, color bombs  
4. **Levels:** Create different GridSettings for various levels
5. **Menus:** Main menu, pause menu, game over screen
6. **Mobile:** Touch controls and mobile optimization
7. **Polish:** Better graphics, animations, effects

## FINAL CHECKLIST

Before considering setup complete:
- ✅ 8x8 grid generates correctly
- ✅ Tiles can be selected (yellow highlight)
- ✅ Adjacent tiles can be swapped  
- ✅ 3+ matches are detected and removed
- ✅ Tiles fall down to fill gaps
- ✅ New tiles spawn from top
- ✅ Score increases with matches
- ✅ UI updates correctly
- ✅ No console errors
- ✅ Runs at 60 FPS

**Congratulations! Your Match 3 game is now fully functional in Unity! 🎉**
