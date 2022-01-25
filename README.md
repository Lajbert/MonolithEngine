# Monolith Engine

# My website
Mostly focusing on MonoGame in general and my personal developer experiences:  
https://lajbert.github.io/

# Disclaimer: 
The engine is very young and I have a lot of documentation, cleanup and refactoring work ahead of me to make it community-friendly. At this point, most commits go directly to the master so it changes frequently. If there will be a community around the engine, this will change. Originally I wasn't planning to open up the source code just yet, but I did it because of Ludum Dare 48, where I have to provide the source code of my submission. The code will undergo several improvements and cleanups in the next weeks/months. I am a beginner in video game development and this is my learning project, so if you find anything that could be improved or should be done better/differently, I am very happy to hear about it. Also, I am happy to recieve any feedback in general.  
**But!** I would not recommend starting your project with my engine at this point yet, there will be a lot of code and design changes as the engine evolves, and I can't guarantee backwards compatibility! If you're interested in it, use it to play around with or learn form it.  
As soon as I feel the engine got it's mostly final shape and ready for public use, I will make official release.

# Sample platformer game:
![ForestLevel](https://img.itch.zone/aW1hZ2UvMTAyMDA1MS81ODE2MDU4LmdpZg==/original/rC%2BG5S.gif)  
![IceLevel](https://img.itch.zone/aW1hZ2UvMTAyMDA1MS81ODE2MDU5LmdpZg==/original/UghiPj.gif)

Windows/MacOSX executable: https://lajbert.itch.io/platformer-demo  
Android PlayStore: https://play.google.com/store/apps/details?id=com.lajos.monolith.platformer  
iPhone AppStore: url coming soon, it's currently in testing phase! For more information, head over the [project page](https://github.com/Lajbert/MonolithEngine/projects), or deploy the code to your phone (see below).  
The shared source code of the game can be found under GameSamples\PlatformerGame\PlatformerNetStandard  
To run on desktop, use the project under GameSamples\PlatformerGame\PlatformerOpenGL  
To run on Android, use the project under GameSamples\PlatformerGame\PlatformerAndroid  
To run on iOS, use the project under GameSamples\PlatformerGame\PlatformerIOS  

# Oh no, not another game engine!
There are so many game engines out there already, why making another one?  
First of all, it exists because I love coding and video games! I have a couple of 2D game ideas in my head and the possible choices of engines was narrowed down to Unity only.
The rest were either "too much" (like UE4), not entirely cross-platform or simply expensive :) . When I was playing around with Unity, I quickly realized that
since I'm the type of person who rather goes to the source code than the documentation, this will not be my preferred choice, so I decided to code my own engine
with the feature set that I will actually use. During the first hours of the development, I already realized that for me, working on the engine is at least as fun as 
working on a specific game. I've chosen to utilize [MonoGame](https://www.monogame.net/) framework to ensure that the engine is cross-platform and low level enough for my needs.

Let me get this out of the way: I shared this engine with the public because I'd be happy to see people learning from it or maybe even help me to improve it, but this engine is not designed specifically for the public, but rather for myself to release my games with it.  
This means features will come according to my needs (or interests :) ) and changes might (and probably will) happen as the engine evolves. Even though it's a "take it or leave it" kind of thing, I will still provide a few game sample games for desktop and Android to showcase what the engine can do and how to make games with it.

# What is this exactly?
This is the alpha version of a simple, [axis aligned](https://en.wikipedia.org/wiki/Axis-aligned_object) 2D game engine that encapsulates the common functionalities that most 2D games need. Is it as comprehensive as other major engines out there? Definitely not. Is it small, fast, lightweight, free and easy to understand? Yes!  

Features so far:
* 2D Animations with state machines (auto-playing animations based on the entity's state, really convenient!)
* Automatic sprite/spritehseet importer with texture caching and bounding box (circle and rectangle) generator
* AI with state machines (behavior trees coming soon)
* TileGroups with automatic texture merging (very useful for external map editors!)
* Layers (for parallax scrolling, background, foreground, etc) with optimized Y-sorting
* 2D camera with the usual functionalities (scroll, zoom, follow, shake, etc.) with dual camera support (horizontal and vertical split)
* Fixed timestep for game logic updates, interpolation for in-between frames (makes the physics predictable and easy on the CPU, while still having butter smooth movements)
* [LDtk](https://ldtk.io/) map editor support ([Tiled](https://www.mapeditor.org/) support is also planned for future)
* 2D grid based static environmental collisions (for dynamic objects colliding with static objects like ground, walls and other static environment colliders)
* dynamic circle and box collisions (for dynamic objects colliding with each other, like the hero character colliding with enemies, pickups, traps, etc.)
* Triggers (Box trigger supported, Circle trigger coming soon) for detecting that an object inside an area, like the hero character entering a certain part of the map that opens a door, etc.
* Raycasting with 2 different implementation: Bresenham line and line-line intersection (for various purposes like AI, graphics effects, etc)
* Keyboard and controller input handling
* Utilities like Timer, math, primitives, etc.
* Basic audio support
* Asset management
* Scene management
* Basic UI capabilites (with touch input support), custom text renderer to use PNG fonts
* Gravity for platformer-type games (can be turned off to make a top-down game)

Many more features are planned, including object pooling, particle system, pathfinding, ease functions, automatic raycasting improvements, more advanced map editor support, native Aseprite support, basic shaders and other juicy stuff, stay tuned!

# Who should use this engine?
At this point, no one besides me, as the engine will most likely still change :) For now, I only recommend to browse the code and play around with it to get inspired or maybe even learn from it.

# How stable is the engine?
It's very young and there are lots of things still to be done, including important features, fixes, some refactoring, cleanups, unit tests, etc. Also, major redesigns can be expected as I the need arises while developing my game.
Once I've released successfully released my first game with it, I will declare it stable and make the first release version. It's constantly being developed and I have high hopes that it's going in the right direction.

**Important!** Please note that the assets used for the sample platformer game has their own licences. Please refer to the individual assets for more information:  
https://pixelfrog-assets.itch.io/  
https://rephil.itch.io/  
https://www.gamedevmarket.net/asset/ui-pixel-assets-4/  
https://www.playonloop.com/2016-music-loops/chubby-cat/  
https://www.fontspace.com/roboto-remix-font-f26577  
https://egordorichev.itch.io/key-set

If you are interested in knowing more about the engine, feel free to contact me.
