# FlowerWaltz
*Imagine being a kid again. It is an early autumn, light is cascading in the tree branches, the birds are singing and roads that once laid bare are covered in a blanket of leaves. You watch in awe as a gust of wind carries a handful of leaves in a sudden rush, forming violent swirls – like a tempest – but eventually subsiding. You decide that you are, in this case bending the will of nature, tempted by your imagination; you are controlling the leaves!*

In this project we demonstrate an application that allows a performer to control simulated leaves inside a 3D environment using real-time motion capture. The experience encourages the performer to move and explore embodied interactions with both visuals and a generative soundscape. Bounding boxes and flow descriptors were primarily used as a way to control parameters i.e. particle behavior and note pitch.

We decided to do the core of our implementation in Unity game engine, and furthermore, we decided to use the SmartSuit Pro to capture motion data in real-time. A generative ambient soundscape was implemented inside Pure Data and interfaces with Unity over Open Sound Control (OSC).
