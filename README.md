# UniRxExam
[![Unity Version](https://img.shields.io/badge/Unity-2019.3.4-999.svg?logo=unity&style=popout)](https://unity.com/)


## Requirements

⬇️ Needs import assets below.

| Asset Name | Version | Links |
|:--|:--|:--|
| UniRx | 7.1.0 | [Unity Asset Store](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276), [Github](https://github.com/neuecc/UniRx) |
| Zenject | 9.1.0 | [Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/extenject-dependency-injection-ioc-157735), [Github](https://github.com/modesttree/Zenject) |
| "Unity-Chan!" Model | 1.2.1 | [Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/unity-chan-model-18705), [Original Site](https://unity-chan.com/) |

## Usage

### Fix build error on AutoBlink.cs
1. Open `AutoBlink` script in Unity-chan! Model
2. Delete `using System.Security.Policy;`
