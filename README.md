# No Click, Sherlock

A short narrative cybersecurity game developed with the University of Groningen. The player explores a digital world as a lost piece of data, speaks with NPCs and completes three minigames. Dialogue choices affect later challenges.

The brief was to make common cybersecurity risks approachable for university staff, including people who do not normally play games. We designed the experience for a short session of roughly fifteen minutes.

[Case study](https://vladut-andrei-lambru.github.io/projects/no-click-sherlock/)

## My role

I was the lead programmer in a six-person team over fifteen weeks. I implemented the core Unity/C# gameplay, movement, NPC dialogue, minigames, UI, animations, cutscenes, saves and scene transitions, and managed the GitHub workflow.

## Gameplay systems

- **Exploration and dialogue:** point-and-click movement and NPC conversations connect the game’s scenes. Shared game state carries decisions into later sections.
- **Platform challenge:** automatic bouncing, procedurally placed platforms and height-based progress. The challenge changes according to earlier decisions.
- **Block puzzle:** players place shapes on a grid and clear completed rows or columns. Clues connect the puzzle to the investigation.
- **Password investigation:** players collect clues and assemble word, number and symbol components to find the required password. This is a game puzzle, not a real password-cracking tool.
- **Game flow:** scene transitions, saved state and cutscenes keep the individual sections connected.

## Repository layout

The Unity project is in `CyberSecuirty-InfraRED/`. The spelling is the original folder name.

```text
CyberSecuirty-InfraRED/
├── Assets/
├── Packages/
└── ProjectSettings/
```

Open that folder in Unity Hub using **Unity 6000.3.8f1**. The original minigame folders include `MG_Blocks`, `MG_DoodleJump` and `MG_Grandma`.

This repository preserves the team’s student project. Third-party assets retain their original licences.

[Portfolio](https://vladut-andrei-lambru.github.io/) · [v.lambru@st.hanze.nl](mailto:v.lambru@st.hanze.nl)
