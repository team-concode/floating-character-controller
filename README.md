# Floating Character Controller

A floating capsule character controller made in Unity 3D.

[![Floating Character Controller](http://img.youtube.com/vi/bHZA_pdb9E0/0.jpg)](https://youtu.be/bHZA_pdb9E0)


## References
This code is implemented based on the technical blog video of the game "Very Very Valet."

[![Very Very Valet](http://img.youtube.com/vi/qdskE8PJy6Q/0.jpg)](https://youtu.be/qdskE8PJy6Q)

I referred to the following article for the implementation of the jumping mechanism and the platform in the code.

[stylised-character-controller](https://github.com/joebinns/stylised-character-controller)


## Features
### Movement
- Fast responsive character movement
- Smooth movement on undulating surfaces
- Avoiding anomalies caused by collider collisions

### Jump
- Coyote Jump
- Jump buffering
- Jump height proportional to the duration of button press
- Jump state event callbacks

### Not implemented
Since the code was created specifically for a pixel art game, certain parts were excluded from the implementation. The missing parts are as follows:

- Capsule upright updating 
- Downward forces 
- Transform forwarding
