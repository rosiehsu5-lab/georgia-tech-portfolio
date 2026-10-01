# Pengu Delivery

## Overview

Pengu Delivery is a third-person 3D Unity game created by a Georgia Tech student team. Players guide a penguin through a winter environment, collect fish, use checkpoints, cross hazardous ice, avoid seal enemies, and complete a delivery route.

The project combines character control, environmental interaction, enemy behavior, UI, audio events, menus, scoring, and level progression in a cohesive playable experience.

## My contribution

My Git history shows contributions across onboarding, narrative presentation, reusable scene tooling, gameplay flow, and interactive systems.

### Storyline system

I improved the opening storyline so narrative content could be presented as a sequence of controlled slides rather than a static screen. The controller manages the current step, advances only when allowed, and transitions cleanly into gameplay.

The flow was designed to prevent accidental skips and make the relationship between story, instructions, and gameplay easier to understand.

### Interactive tutorial

I implemented tutorial progression that introduces mechanics in stages. One later improvement added a dedicated double-jump step and changed the interface so the Continue prompt appears only on the final storyline slide.

This required coordinating player actions, tutorial state, UI visibility, and scene progression rather than treating instructions as independent text.

### Unity Editor automation

I built Editor-side setup tools for storyline and tutorial scenes. These tools automate repeated configuration work and reduce the chance that a scene is missing required objects, references, or hierarchy structure.

Editor automation was especially useful in a team environment: instead of relying on every contributor to reproduce a scene manually, the setup logic encoded important configuration decisions in a repeatable tool.

### Demo-station helper

I added a reusable demo-station component and documented the intended demo-scene structure. The goal was to make individual mechanics easier to present, test, and discuss without requiring a reviewer to play through the entire game.

### Gameplay and navigation flow

My earlier commits also touched player and menu flow, including checkpoints, bonus-fish interactions, start and pause menus, credits, game-over behavior, score presentation, timers, scene switching, and Unity build settings.

## Technical architecture

The game uses Unity scenes to separate storyline, tutorial, gameplay difficulty levels, success, credits, and game-over states. Reusable prefabs represent gameplay objects such as the player, fish, checkpoints, breakable ice, enemies, and menus.

C# components handle:

- Player input and movement
- Noise and sound-event propagation
- Seal patrol and player detection
- Collectible fish and bonus-fish behavior
- Checkpoint activation
- Breakable environmental objects
- Timers and score state
- Pause, success, and game-over transitions
- Storyline and tutorial state machines
- Camera behavior and scene switching

## Team and attribution

Pengu Delivery was a collaborative project. The full repository contains contributions from multiple Georgia Tech students as well as third-party Unity asset packages. This public case study documents my contributions without claiming sole authorship or redistributing material whose publication rights have not been confirmed.

Institutional repository: `https://github.gatech.edu/The-Penguins/pengu-delivery`

