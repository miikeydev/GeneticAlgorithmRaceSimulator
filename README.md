# 🚗 Genetic Algorithm Car Simulation

This Unity project showcases a car controlled by a neural network optimized with a genetic algorithm. The car improves its behavior over generations by learning to navigate checkpoints and avoid obstacles.

## 📋 Table of Contents

- [Features](#features)
- [Repository Structure](#repository-structure)
- [Setup](#setup)
- [How It Works](#how-it-works)
- [Configuration Options](#configuration-options)
- [Requirements](#requirements)
- [License](#license)

## 🌟 Features

- **Neural Network Controller**: Flexible network architecture for car control based on raycast data.
- **Genetic Algorithm**: Evolutionary optimization of neural network weights through selection, crossover, and mutation.
- **Checkpoint Fitness Scoring**: Cars earn rewards for passing checkpoints and avoiding collisions.
- **Modular Components**: Easily adjustable parameters for custom network structures, movement, and GA settings.

## 📂 Repository Structure

- **Scripts**:
  - `TruckController`: Controls car movement based on neural network output.
  - `NeuralNetController`: Manages neural network setup, forward propagation, and action selection.
  - `GeneticAlgorithm`: Executes selection, crossover, and mutation of weights.
  - `CheckpointManager` & `Checkpoint`: Manages checkpoint tracking and fitness scoring.
  - `WeightManipulation`: Saves and loads neural network weights across generations.
- **Assets**: Unity assets for car models, checkpoints, and environments.

## ⚙️ Setup

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/your-repo-name.git
