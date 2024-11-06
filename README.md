# 🚗 Genetic Algorithm Race Simulator

Welcome to the **Genetic Algorithm Race Simulator**! This Unity project implements a car simulator where vehicles are controlled by neural networks optimized through a genetic algorithm. Cars learn to navigate checkpoints, avoid obstacles, and improve their performance over generations.

## 📋 Table of Contents

- [Features](#features)
- [Repository Structure](#repository-structure)
- [Setup](#setup)
- [How It Works](#how-it-works)
- [Configuration Options](#configuration-options)
- [Requirements](#requirements)
- [License](#license)

## 🌟 Features

- **Neural Network Control**: A flexible network architecture that interprets raycast data to steer the car.
- **Genetic Algorithm Optimization**: Implements selection, crossover, and mutation to improve neural network weights generation by generation.
- **Checkpoint-Based Fitness Scoring**: Cars earn points by passing checkpoints and avoiding collisions.
- **Configurable Components**: Customizable parameters for network structure, car dynamics, and genetic algorithm settings.

## 📂 Repository Structure

- **Scripts**:
  - `TruckController`: Controls car movement based on neural network output.
  - `NeuralNetController`: Manages neural network initialization, forward propagation, and action selection.
  - `GeneticAlgorithm`: Executes selection, crossover, and mutation on neural network weights.
  - `CheckpointManager` & `Checkpoint`: Handles checkpoint tracking and fitness scoring.
  - `WeightManipulation`: Saves and loads the best weights for analysis.
- **Assets**: Unity assets including models for cars, checkpoints, and environments.

## ⚙️ Setup

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/miikeydev/GeneticAlgorithmRaceSimulator.git
   
2. Press **Play** in the Unity Editor to start the simulation.


## 🛠️ How It Works

- **Initialization**: Each car is assigned random weights for its neural network in the first generation.
- **Fitness Evaluation**: Cars accumulate scores by reaching checkpoints and avoiding boundary collisions.
- **Evolution Process**:
  - **Selection**: Top-performing cars are chosen as parents.
  - **Crossover**: New cars are created by blending parent weights.
  - **Mutation**: Random mutations introduce variations to prevent performance stagnation.
- **Saving & Loading**: Best-performing weights are saved for further use and analysis.

## 🔧 Configuration Options

- **Neural Network**:
  - Set `InputSize`, `HiddenLayerSize`, and `OutputSize` in `NeuralNetController`.
- **Genetic Algorithm**:
  - Adjust `mutationRate` in `GeneticAlgorithm`.
  - Set `numberOfIndividuals` in `GameManager`.
- **Checkpoints**: Customize checkpoint positions directly in the Unity scene.

## 🖥️ Requirements

- Unity 2022.3.9 or later.
