Sense of Escape — Haptic-Thermal Feedback Glove Integrated with Unity

Graduation project — a Unity-based 2D escape room game controlled by a custom wearable glove that delivers real-time vibration and thermal feedback synchronized with in-game events.

Overview

Sense of Escape explores whether multimodal haptic and thermal feedback, combined with gesture-based control, can improve immersion and presence in a 2D game compared to traditional keyboard input. Players wear a glove fitted with flex sensors and an IMU to move and interact, while an ESP32 microcontroller drives vibration and heat feedback based on real-time events sent from Unity.

The full software stack — Unity gameplay systems, ESP32-Unity serial protocol, and the PHP/MySQL backend — was designed and implemented as part of this project. Hardware design (sensor/actuator circuitry, enclosure) was developed by a teammate.

Features


Three escape room levels, each showcasing a different feedback modality:

Vase Puzzle Room — find a hidden key by breaking vases; correct guesses trigger a vibration pulse, wrong ones a burst and a scoring penalty.
Combat Room — A*-pathfinding enemy with a simple two-state AI; player fights with a sword, taking damage triggers hit vibration.
Frozen Room — picking up a torch gradually activates thermal (heat) feedback that melts an icy door.



Dual input modes: keyboard/mouse for development and testing, or the physical glove for the validated experience — switchable from the main menu.
Custom wearable glove: 3 flex sensors (finger gestures), MPU6050 IMU (movement via hand tilt), a vibration motor, and a resistive heating element with DS18B20 temperature monitoring for safety.
Text-based serial protocol between ESP32 and Unity (115200 baud) — e.g. W/A/S/D/IDLE for movement, ACT:E / ACT:Q / ACT:SPACE for actions, VIB:PULSE / VIB:BURST / VIB:HIT and HEAT:ON / HEAT:OFF for feedback.
A* pathfinding for enemy movement, built on a runtime-generated walkability grid (visualized via Gizmos during development).
Session tracking and scoreboard: student login, per-level completion time and score (with penalties for wrong vase breaks), stored in MySQL via a PHP API and displayed on a ranked scoreboard scene.


Architecture

Player  ⇄  Unity (game logic, feedback triggers, serial dispatcher)  ⇄  Wearable Glove (ESP32, sensors, actuators)
                        │
                        ▼
              PHP API  ⇄  MySQL (session results, scoreboard)


Game engine: Unity 6, C#
Communication layer: SerialController (transport) + FeedbackManager (single point of contact for haptic/thermal commands — gameplay scripts never talk to the serial port directly)
Backend: PHP (XAMPP/Apache) + MySQL, accessed from Unity via UnityWebRequest; no direct DB connection from the game client
Firmware: ESP32 (Arduino), threshold-based sensor interpretation, non-blocking vibration/heat timing via millis()


Tech Stack

Unity 6 · C# · ESP32 (Arduino/C++) · PHP · MySQL · XAMPP · TextMeshPro · Cinemachine

Hardware

ComponentRoleESP32Main controller — reads sensors, drives actuators, serial link to Unity3× Flex sensor (index/middle/ring)Finger-gesture input for interact/secondary/attack actionsMPU6050 (I2C)Hand-tilt based directional movementVibration motorHaptic feedback (pulse / burst / hit patterns)Custom resistive heater + IRFZ44N MOSFETThermal feedback for the frozen-door sceneDS18B20Temperature safety monitoring near the heater

Repository Contents

This repo contains the Unity C# scripts and PHP backend for the project:


Gameplay: PlayerMovement, PlayerInventory, PickupItem, PickupVase, DoorInteraction, WrongBreakCounter
Combat & pathfinding: EnemyFollow, EnemyAttack, EnemyHealth, EnemyManager, AStarGrid, Node, PlayerSwordAttack
Hardware integration: SerialController (MPU6050.cs), FeedbackManager
Session & UI: GameManager, LoginManager, DatabaseManager, PromptManager, LevelTimer, DeathPanelManager, MenuManager, EndMenuManager, ScoreboardManager, IntroMessage
Backend: login.php, save_result.php, scoreboard.php
