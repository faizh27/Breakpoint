# Breakpoint

A 2D roguelike tennis game for Steam inspired by **Hangtime** (volleyball). Fast-paced rallies with custom ball physics and precise, tunable controls. Aiming to be the best tennis game on mobile.

## Features

- **Custom Ball Physics** — Hand-tuned parabolic trajectories instead of rigidbody constraints. Full control over apex height, flight duration, bounce damping.
- **Top-Down Tennis** — Court reads instantly. Depth sold through shadow scaling and sprite size changes.
- **AI Opponent** — Tracks the ball, predicts landing positions, returns with varying shot types (flat, normal, lob).
- **Tennis Scoring** — 0/15/30/40/game with deuce support planned.
- **Input System** — Multi-device support (keyboard, gamepad, touch, XR).

## Current State

**Working:**
- Player movement and swing hitbox
- Ball flight and bouncing
- AI opponent with basic shot selection
- Serve system
- Score tracking and game flow
- Court boundary detection

**Coming Next:**
- Timing tiers (perfect/good/ok hits with different shot qualities)
- Power shots and special abilities
- Roguelike progression (procedural opponents, unlocks)
- Court variety and visual polish
- Mobile-optimized UI

## How to Play

**Controls:**
- **WASD / Arrow Keys** — Move
- **Space** — Serve (when prompted) / Swing
- **1/2/3** — Test shot types (debug, will be removed)
- **R** — Reset ball (debug)

**Gamepad:**
- **Left Stick** — Move
- **A / X Button** — Swing
- **D-Pad** — Navigate (UI)

**Goal:** Win rallies by returning the ball before your opponent. Stay in bounds, time your shots, outlast the AI.

## Tech Stack

- **Engine:** Unity 2022+
- **Language:** C#
- **Physics:** Custom (no Rigidbody for ball flight)
- **Input:** Unity Input System
- **UI:** TextMeshPro

## Getting Started

1. Clone the repo
2. Open in Unity 2022 or later
3. Load `SampleScene`
4. Attach scripts:
   - `GameManager` → empty GameObject in scene
   - `CourtSystem` → empty GameObject
   - `UIManager` → empty GameObject
   - `AIOpponent` → duplicate of Player, remove PlayerController
5. Wire up references in inspector (see script comments)
6. Play and iterate

## File Structure

```
Assets/
├── Scripts/
│   ├── BallController.cs       — Ball physics & visuals
│   ├── PlayerController.cs     — Player input & swinging
│   ├── AIOpponent.cs           — AI movement & shot logic
│   ├── GameManager.cs          — Game state & scoring
│   ├── CourtSystem.cs          — Boundary detection
│   └── UIManager.cs            — Score display & game over
└── Scenes/
    └── SampleScene.unity
```

## Contributing

Early-stage project. Feedback welcome — open an issue or PR.

## License

MIT
