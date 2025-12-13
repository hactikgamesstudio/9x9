# 9x9 Documentation Index

**Last Updated**: December 12, 2025  
**Project**: 9x9 Cube Maze Survival Game  
**Unity Version**: 6000.2.10f1

---

## 📖 Overview

This documentation covers all aspects of the 9x9 project, from initial setup to advanced development. Use this index to quickly find the information you need.

---

## 🧭 How to Use This Documentation (Start → Finish)

Follow this order the first time you touch the project:

| Step | Purpose | Read This |
|------|---------|-----------|
| 0. Product overview | What we’re building and for whom | [Project README](../README.md), [Game Overview](GameDesign/GAME_OVERVIEW.md) |
| 1. Install & set up | Clone, install Unity, configure input | [Quick Start](Setup/QUICK_START.md) (experienced) / [Full Setup](Setup/FULL_SETUP_GUIDE.md) (beginners) |
| 2. Understand structure | Know where code/assets live | [Project Structure](PROJECT_STRUCTURE.md) |
| 3. Run the game | Launch metagame, start a session | [Cube Game Setup](Setup/CUBE_GAME_SETUP.md), [Singleplayer Setup](SINGLEPLAYER_SETUP.md) |
| 4. Build features | Add gameplay, UI, bots | [Development Roadmap](ROADMAP.md), [Controllers/Input](Setup/CONTROLLER_SETUP.md), [UI Input Improvements](Setup/UI_INPUT_IMPROVEMENTS.md) |
| 5. Test & troubleshoot | Fix common errors | [Troubleshooting](Setup/TROUBLESHOOTING.md) |
| 6. Ship | Build and verify | [Build Plan](Development/BUILD_PLAN.md), [Code Quality Report](Development/CODE_QUALITY_REPORT.md) |

---

## 🚀 Start Here

- [Project README](../README.md) — high-level overview, tech stack, contribution basics
- Choose your setup path:
	- Experienced: [Setup/QUICK_START.md](Setup/QUICK_START.md)
	- Beginners / Godot porters: [Setup/FULL_SETUP_GUIDE.md](Setup/FULL_SETUP_GUIDE.md)

---

## 🔍 Understand the Project

- [Project Structure](PROJECT_STRUCTURE.md) — where scripts, prefabs, and scenes live
- [Game Overview](GameDesign/GAME_OVERVIEW.md) — vision, modes, target audience (filled)
- [Roadmap](ROADMAP.md) — milestones and status

---

## 🛠️ Build and Run

- Core gameplay setup: [Setup/CUBE_GAME_SETUP.md](Setup/CUBE_GAME_SETUP.md)
- Singleplayer/bots: [SINGLEPLAYER_SETUP.md](SINGLEPLAYER_SETUP.md)
- Input and controllers: [Setup/CONTROLLER_SETUP.md](Setup/CONTROLLER_SETUP.md), [Setup/UI_INPUT_IMPROVEMENTS.md](Setup/UI_INPUT_IMPROVEMENTS.md)
- Flashlight/pickups extras: [Setup/FLASHLIGHT_SETUP.md](Setup/FLASHLIGHT_SETUP.md), [Setup/CREATE_SCRIPTABLEOBJECTS.md](Setup/CREATE_SCRIPTABLEOBJECTS.md)

---

## 🎨 Design & Reference

- Game design: [GameDesign/ART_DIRECTION.md](GameDesign/ART_DIRECTION.md), [GameDesign/MECHANICS.md](GameDesign/MECHANICS.md)
- Template/MVC reference: [References/README.md](References/README.md)
- Cleanup rationale: [REDUNDANT_FILES_EXPLAINED.md](REDUNDANT_FILES_EXPLAINED.md), [CLEANUP_PLAN.md](CLEANUP_PLAN.md), [CLEANUP_SUMMARY.md](CLEANUP_SUMMARY.md)

---

## 🧪 Test, Troubleshoot, Ship

- Build & release: [Development/BUILD_PLAN.md](Development/BUILD_PLAN.md), [Development/CODE_QUALITY_REPORT.md](Development/CODE_QUALITY_REPORT.md)
- Issues: [Setup/TROUBLESHOOTING.md](Setup/TROUBLESHOOTING.md)
- Integration checks: [Development/INTEGRATION_TEST_CHECKLIST.md](Development/INTEGRATION_TEST_CHECKLIST.md)

---

## 📝 Session Notes

- [SessionNotes/2025-11-13.md](SessionNotes/2025-11-13.md) — initial port notes
- Add new notes per session in `Documentation/SessionNotes/` (date-based filenames)

---

## 📅 Documentation Roadmap

- High priority: add API reference (CubeGame classes, RoomGenerator, InventorySystem); document HUD system; add session notes for Nov 17, 2025.
- Medium: expand Art Direction; create networking guide (Netcode patterns); document testing procedures; add performance optimization guide.
- Low: video tutorials; automated API docs; multi-language support; interactive examples.

---

## 🔄 Keeping Docs Fresh

- Update docs when adding features, fixing common issues, or changing architecture.
- Keep consistent Markdown formatting, include dates/versions, and add code samples where useful.

**Documentation Maintainer**: Keith Baker  
**Last Major Revision**: December 12, 2025 (start-to-finish reflow)

**Thank you for keeping our documentation up to date! 📚**
