# Monolith Engine

# My website
Mostly focusing on MonoGame in general and my personal developer experiences:  
https://lajbert.github.io/

# Disclaimer: 
The engine is very young and I have a lot of documentation, cleanup and refactoring work ahead of me to make it community-friendly. At this point, most commits go directly to the master so it changes frequently. If there will be a community around the engine, this will change. Originally I wasn't planning to open up the source code just yet, but I did it because of Ludum Dare 48, where I have to provide the source code of my submission. The code will undergo several improvements and cleanups in the next weeks/months. I am a beginner in video game development and this is my learning project, so if you find anything that could be improved or should be done better/differently, I am very happy to hear about it. Also, I am happy to recieve any feedback in general.

# Sample platformer game:
![ForestLevel](https://img.itch.zone/aW1hZ2UvMTAyMDA1MS81ODE2MDU4LmdpZg==/original/rC%2BG5S.gif)  
![IceLevel](https://img.itch.zone/aW1hZ2UvMTAyMDA1MS81ODE2MDU5LmdpZg==/original/UghiPj.gif)

Windows executable: https://lajbert.itch.io/platformer-demo  
Android version: PlayStore url coming soon! For more information, head over the [project page](https://github.com/Lajbert/MonolithEngine/projects), or deploy the code to your phone (see below).  
The shared source code of the game can be found under GameSamples\PlatformerGame\PlatformerNetStandard  
To run on desktop, use the project under GameSamples\PlatformerGame\PlatformerOpenGL  
To run on Android, use the project under GameSamples\PlatformerGame\PlatformerAndroid  

# Oh no, not another game engine!
There are so many game engines out there already, why making another one?  
First of all, it exists because I love coding and video games! I have a couple of 2D game ideas in my head and the possible choices of engines was narrowed down to Unity only.
The rest were either "too much" (like UE4), not entirely cross-platform or simply expensive :) . When I was playing around with Unity, I quickly realized that
since I'm the type of person who rather goes to the source code than the documentation, this will not be my preferred choice, so I decided to code my own engine
with the feature set that I will actually use. During the first hours of the development, I already realized that for me, working on the engine is at least as fun as 
working on a specific game. I've chosen to utilize [MonoGame](https://www.monogame.net/) framework to ensure that the engine is cross-platform and low level enough for my needs.

Let me get this out of the way: I shared this engine with the public because I'd be happy to see other people use it, learn from it or maybe even help me to improve it, but this engine is not designed specifically for the public, but rather for myself to release my games with it.  
This means features will come according to my needs (or interests :) ) and changes might (and probably will) happen as the engine evolves. Even though it's a "take it or leave it" kind of thing, I will still provide a few game sample games for desktop and Android to showcase what the engine can do and how to make games with it.

# What is this exactly?
This is a simple, [axis aligned](https://en.wikipedia.org/wiki/Axis-aligned_object) 2D game engine that encapsulates the common functionalities that most 2D games need. Is it as comprehensive as other major engines out there? Definitely not. Is it small, fast, lightweight, free and easy to understand? Yes! 

Features so far:
* 2D Animations with state machines (auto-playing animations based on the entity's state, really convenient!)
* Automatic sprite/spritehseet importer with texture caching and bounding box (circle and rectangle) generator
* AI with state machines (behavior trees coming soon)
* TileGroups with automatic texture merging (very useful for external map editors!)
* Layers (for parallax scrolling, background, foreground, etc) with optimized Y-sorting
* 2D Camera with the usual functionalities (scroll, zoom, follow, shake, etc.)
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
* Basic UI capabilites (text boxes and interactive UI elements that reacts to mouse/keyboard or touch input)
* Gravity for platformer-type games (can be turned off to make a top-down game)

Many more features are coming including object pooling, particle system, pathfinding, ease functions, automatic raycast collision optimization (merging smaller lines into 1 big line), more advanced map editor support, native Aseprite support, basic shaders and other juicy stuff, stay tuned!

# How stable is the engine?
It's very young and there are lots of things still to be done, including important features, fixes, some refactoring, cleanups, unit tests, etc. I it's still in alpha version, but thanks to the lockdown, I'm making a good progress with it :). I always finish one feature fully to the best of my knowledge before starting a new one, so whatever is there is usable, but everything is still subjected to changes and improvements as the engine evolves and bugs uncovered.  
Once I've released successfully released my first game with it, I will declare it stable and make the first release version.

# Who should use this engine?
Although I'm striving for simplicity, this engine is not made for beginners, especially not for people who rely on user interfaces like the one Unity has. It's for people who don't mind adjusting pivots and offsets from the code and prefers checking out the source to reading endless documentations.

# Can I make a game like [...] with this engine?
Being an axis-aligned engine in itself has some limitations. There is no complex, accurate physics engine (like Box2D) implemented, but unless you're making a physics-heavy game, believe me, you won't need it. 95% of the games don't need accurate physics (in fact, it can easily make them completely unenjoyable), even a complex game like [Dead Cells](https://dead-cells.com/) is [built](https://deepnight.net/tutorial/a-simple-platformer-engine-part-1-basics/) using similar feature set that you can find in this engine. Long story short: do you want make a game like [Dead Cells](https://dead-cells.com/), [Super Mario](https://en.wikipedia.org/wiki/Super_Mario), [Factorio](https://factorio.com/), [Faster Than Light](https://subsetgames.com/ftl.html), [Candy Crush](https://en.wikipedia.org/wiki/Candy_Crush_Saga) or [Breakout](https://en.wikipedia.org/wiki/Breakout_(video_game))? Go for it, this engine is specifically designed for games like these. Do you want to make a physics-heavy game like [Angry Birds](https://www.angrybirds.com/games/)? Even though it's not impossible, it would require a lot of work to adapt the engine, and you'll have to implement or integrate a complex physics engine yourself, so you might want to consider alternatives that are a better fit for your goal.

# What do I need to get started?
The more coding experience you have, the better. It doesn't matter if it's Java, C++ or any other OO language as C# will be easy to pick up if you have programming knowledge. Some MonoGame knowledge helps too, but not required. Many of MonoGame's functionalities are already encapsulated in the engine, so unless you're planning to make changes in the engine itself, you're covered. What could be really helpful is understanding code performance, algorithms and data structures.

# How can I use the engine? Any learning materials or tutorials?
Right now there is one [platformer game example](https://lajbert.itch.io/platformer-demo) included in project to get you started, but more examples will be added later for the most typical scenarios and genres. Written and/or youtube tutorials are also planned to make the start easier.
Since the engine is built on top of MonoGame, you'll have to follow the [MonoGame's 'Getting Started' documentation](https://docs.monogame.net/index.html) to setup your environment. Nothing else is needed.  
The project itself if a shared MonoGame project, so you can include it in yours, regardless of the target. Head over to GameSamples\PlatformerGame\ to see the example platformer game for PC and mobile.

# Why should I chose this engine over other major commercial engines out there?
I can't really come up with any catchy selling point apart from the things I already listed above :) It's there if you'd like to try something new and simple, maybe you'll like it, maybe not, maybe you'll just see one or two tricks you like and add it to your toolbox.

# I have a little experience with Unity (or any other game engine), is that going to help me?
It depends. There is definitely a huge amount of transferable knowledge in the game developer industry regardless of the tools, and some things in Monolith engine are done similarly to Unity that might be familiar, but Monolith being a pure 2D engine without it's own UI, most things are done differently. Writing performant code, good level design and "game feel" are universal, but user interface knowledge of different engines will not be helpful, as you have to rely on the code here (except for editing the map, for which you have an [awesome open source](https://ldtk.io/) supported tool).

# What are the supported platforms?
Whatever MonoGame supports, the rest depends on you and your code :) The engine itself is pretty lighweight without any platform-specific code, so runs well on any low-end hardware. Please note that for consoles, you have to be a licenced developer to acquire the respective binaries from the MonoGame team.  

# What games have been released with it?
None commercial yet, hopefully the first will be released in Q3-Q4 2021.

A platformer game sample can be found here:
https://lajbert.itch.io/platformer-demo

I also used the engine for Ludum Dare 48:
https://lajbert.itch.io/mullettime - please note that I only had 48 hours to make a game alone, including graphics and audio, so this doesn't represent the capabilities of the engine :) Please note that the engine's source is packaged next to the game code, so it's basically a snapshot of the engine's status at that time. Please do not use that version of the engine to make games, but the latest version from this Github repo, otherwise you will miss all the developments that happened to the engine since then.


# Am I free to use the engine to make games?
Yes, any way you want, including commercially. The only thing you cannot do is selling the engine itself as it is, but you're free to use it (partially or entirely) to make your own game or game engine, including commercial use. Free means free: no royalties, fees, or any other hidden costs. I'm grateful if you give me credits in your preferred way, but it's also not mandatory. Please keep in mind that by using the engine, you accept that you are using it "as it is" and the creator of the engine is not liable for any damages, losses or any other harm (including, but not limited to financial, personal or any other type of damage) that is coming from using the engine. The user of this engine takes all the risks and responsibilities.

# I want to write my own engine, can I use your code?
Yes, feel free to take parts or the entire source code to your engine and create with it.

# Can I contribute to the engine?
Of course, contact me directly if you'd like to make suggestions or commit some code!

# Can I request features?
If there will be a community around the engine, I will make it possible to request features, but my main focus right now is to implement the features that I wll need for my game(s).

# Any chances of 3D support?
Unlikely. This engine was designed specifically for 2D development and making it 3D would be such a big work that I'd rather write a new engine from scratch.
I am aware 2D and 3D development are similar in many ways, but also different enough that I decide to design my engine specifically for 2D to get the best possible 2D performance and easy, logical use.

# Do you provide support for the engine?
I can't. It's a hobby project currently and I don't have the means to do it. If something is seriously broken, or could be done better than I do, please contact me as it's also in my best interest to get it fixed.

# Do you have a bug tracker somewhere?
Right now, only Github issues :) If there will be a community around the engine, I will have do it properly.

# Is there a timeline for future development?
It's planned, but it depends on the public interest in the engine.
