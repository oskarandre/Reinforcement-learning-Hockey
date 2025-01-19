# Reinforcement Learning Hockey

This project uses Unity's mlAgents and reinforcement learning to train agents to play hockey against each other.
The players are controlled by AI, trained with deep reinforcement learning (DRL) andProximal Policy Optimization (PPO). 
These methods enable the agents to continuously learn and optimize their strategies, resulting in dynamic, adaptive gameplay that mimics human decision-making and teamwork.

Created by [Oskar André](https://github.com/oskarandre/), [Kenton Larsson](https://github.com/KnetusL), [Gustav Andersson](https://github.com/Gusandersson)


## Installation

1. Install Unity from [Unity's official website](https://unity.com/).
2. Install the mlAgents package:
    ```bash
    pip install mlagents
    ```
3. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/Reinforcement-learning-Hockey.git
    ```
4. Navigate to the project directory:
    ```bash
    cd Reinforcement-learning-Hockey
    ```
5. Install venv virtual environment:
    ```bash
    py -3.9 -m venv venv
    ```
6. Activate the virtual environment:
    ```bash
    venv\scripts\activate
    ```
7. Upgrade pip:
    ```bash
    python -m pip install --upgrade pip
    ```
8. Install mlAgents:
    ```bash
    pip install mlagents
    ```
9. Install PyTorch:
    ```bash
    pip3 install torch torchvision torchaudio
    ```
10. Install protobuf:
    ```bash
    pip install protobuf==3.20.3
    ```
11. Install packaging:
    ```bash
    pip install packaging
    ```
12. Install ONNX:
    ```bash
    pip install onnx
    ```

## Usage

1. Open up you unity project.

2. Navigate to the project directory:
    ```bash
    cd Reinforcement-learning-Hockey
    ```
3. Activate the virtual environment:
    ```bash
    venv\scripts\activate
    ```
4. Train agent:
    ```bash
    mlagents-learn AgentController.yaml --run-id=NAMEOFYOURCHOOSING
    ```
5. Press START/PLAY/RUN in Unity and wait for training to finish.

6. Train from another brain:
    ```bash
    mlagents-learn AgentController.yaml --initialize-from NAMEOFOLDRUN --run-id=NAMEOFYOURCHOOSING
    ```
