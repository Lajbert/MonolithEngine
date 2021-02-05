# 2DGameEngine
# Oh no, not another game engine!
There are so many game engines out there already, why making another one?
First of all, it exists because I love coding! I have a couple of 2D game ideas in my head and the possible choices of engines was narrowed down to Unity only.
The rest were either "too much" (like UE4) or not entirely cross-platform. When I was playing around with Unity, I quickly realized that
since I'm the type of person who rather goes to the source code than the documentation, this will not be my preferred choice, so I decided to code my own engine
with the feature set that I will actually use. During the first hours of the development, I already realized that for me, working on the engine is at least as fun as 
working on a specific game. I chose MonoGame framework to ensure that the engine cross-platform and low level enough for my needs.

Let me get this out of the way: I shared this engine with the public because I'd be happy to see other people use it, learn from it or maybe even help me to improve it, but this engine is not designed specifically for the public, but rather for myself to release my games with it. 
This means features will come according to my needs (or interests :) ) and changes might (and probably will) happen as the engine evolves. 

# What is this exactly?
This is a simple 2D game engine that can handle basic tasks that most 2D games need. Is it as comprehensive as other major engines out there? Definitely not. Is it small, fast, lightweight, free and easy to understand? Yes! 

Already working features:
* 2D Animations with state machine with automatic spritehseet importer and texture caching
* TileGroups: automatically merges different textures (like static envionmental textures, background, etc)  into 1 texture in the memory
* Layers (for parallax scrolling, background, foreground, etc)
* [LDtk](https://ldtk.io/) map editor support ([Tiled](https://www.mapeditor.org/) support is also planned for future)
* Grid based box collision physics
* Circle collision physics (AABB box collision physics is coming)
* Raycasting!
* Many more features are coming including ease functions, autoamtic raycast collision optimization (merging smaller lines into 1 big line), AABB box collisions, more advanced map editor support and other juicy stuff, stay tuned!

# How stable is the engine?
It's very young and there are lots of things still to be done, including important features, fixes, some refactoring, unit tests, etc. I always finish one feature fully to the best of my knowledge before starting a new one, so whatever is there is usable, but everyting is still subjected to changes and improvements as the engine evolves and bugs uncovered.

# Who should use this engine?
Although I'm striving for simplicity, this engine is not made for beginners, especially not for people who relies on user interfaces like the one Unity has. It's for people who don't mind adjusting pivots and offsets from the code and prefers checking out the source to reading endless documentations.

# What do I need to get started?
The more coding experience you have, the better. It doens't matter if it's Java, C++ or any other OO language as C# will be easy to pick up if you have programming knowledge. Some MonoGame knowledge helps too, but not required. What could be really helpful is understanding code peformance, algorithms and data structures.

# I have a little experience with Unity (or any other game engine), is that going to help me?
It depends. There is definitely a huge amount of transferrable knowledge in the game developer industry regardless of the tools. Writing performant code, good level design and "game feel"  is universal, but user interface knowledge of different engines will not be helpful, as you have to rely on the code here (expect for editing the map, for which you have an [awesome open source](https://ldtk.io/) supported tool here).

# What are the supported platforms?
Whatever MonoGame supports, the rest depends on you :)

# What games have been released with it?
None yet, hopefully the first will be released in Q3-Q4 2021.

# Am I free to use the engine to make games?
Yes, any way you want, including commercially. The only thing you cannot do is selling the engine itself as it is, but you're free to use it (partially or entirely) to make your own game or game engine, including commercial use. Free means free: no royalties, fees, or any other hidden costs. I'm grateful if you give me credits in your preferred way, but it's also not mandatory. Please keep in mind that by using the engine, you accept that you are using it "as it is" and the creator of the engine is not responsible any damages, losses or any other harm (financial or other) that is coming from using the engine. The user this engines takes all the responsibilities.

# I want to write my own engine, can I use your code?
Yes, feel free to take parts or the entire source code to your engine and create with it.

# Can I contribute to the engine?
It will be possible in the future, the details are currently being figured out. Stay tuned!

# Can I request features?
I am planning to make it possible to request features

# Any chances of 3D support?
While 2.5D support is planned later this year (3D characters on 2D sidescroller or isometric map, like Hades), full 3D support will probably never come. This engine was designed specifically for 2D development and making it 3D would such a big work that I'd rather write a new engine from scratch :) 
I am aware 2D and 3D development are similar in many ways, but also different enought that I decide to design my engine specifically for 2D to get the best possible 2D performance and easy, logical use.
