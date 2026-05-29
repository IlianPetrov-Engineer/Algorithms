<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project
This project was developed as a university assignment for my Algorithms course. The goal of the project was to create an algorithm for generating a dungeon. The generation starts with a master room that is then divided into smaller rooms. After that the rooms are connected with doors. Finally, a graph is constructed using **DFS** (Depth-First Search) and a **NavMesh** is generated to allow the player to move (the player move by clicking on the screen).

<img width="1322" height="445" alt="Graph Complete" src="https://github.com/user-attachments/assets/aa41202e-087c-4b94-861d-52a27f5a935f" />
<img width="2232" height="1046" alt="Assets Placed" src="https://github.com/user-attachments/assets/aefada28-5427-43ea-b98a-3929ae87c0ab" />
<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

* [![Unity][Unity.img]][Unity-url]
* [![C#][C#.img]][C#-url]
* [UnityNavMesh][Unity NavMesh-url]
* [NaughtyAttributes][NaughtyAttributes-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

* Unity 2022.3 LTS or newer
* NaughtyAttributes package installed via the Unity Package Manager

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage
The scene with the dungeon generation is *ProceduralDungeonScene*.
Different aspects of the generation can be configured in the Unity Inspector on the *DungeonGenerator* component:
* Dungeon size - mini(50;50) / small(100;100) / medium(150;150) / large(200;200) / custom
* Max rooms - the maximum number of rooms
* Min width / height - the minimum dimensions of the rooms
* Remove percentage - what percentage of rooms to be removed
* Set seed - enable a seed generation (useful of replication)
* Time per room - the speed of the generation (low time may cause lag)
* Automatic generation - toggle auto-generation on "Play"

<p align="right">(<a href="#readme-top">back to top</a>)</p>

[Unity.img]: https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white
[Unity-url]: https://unity.com/
[C#.img]: http://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=unity&logoColor=white
[C#-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[Unity NavMesh-url]: https://docs.unity3d.com/ScriptReference/AI.NavMesh.html
[NaughtyAttributes-url]: https://github.com/dbrizov/NaughtyAttributes
