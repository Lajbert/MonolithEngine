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
* 2D Animations with state machine with automatic spritehseet loader and texture caching
* TileGroups: automatically merges different textures (like static envionmental textures, background, etc)  into 1 texture in the memory
* Layers (for parallax scrolling, background, foreground, etc)
* [LDtk](https://ldtk.io/) map editor support ([Tiled](https://www.mapeditor.org/) support is also planned for future)
* Grid based box collision physics
* Circle collision physics (AABB box collision physics is coming)
* Raycasting!

# Who should use this engine?
Although I'm striving for simplicity, this engine is not made for beginners, especially not for people who relies on user interfaces like the one Unity has. It's for people who don't mind adjusting the pivot and offsets from the source code and prefers checking out the source to reading endless documentations.

# What do you need to get started?
The more coding experience you have, the better. It doens't matter if it's Java or C++ as C# will be easy to pick up if you have any OO programming language knowledge. Some MonoGame knowledge helps too, but not required.

# What are the supported platforms?
Whatever MonoGame supports, the rest depends on you :)

# What games have been released with it?
None yet, hopefully the first will be released in Q3-Q4 2021.

# Am I free to use the engine?
Yes, any way you want, including commercially. The only thing you cannot do is selling the engine itself as it is, but you're free to use it (partially or entirely) to make your own game or game engine, including commercial use. Free means free: no royalties, fees, or any other hidden costs for you. I'm grateful if you give credits in your preferred way, but it's also not mandatory. Please keep in mind that by using the engine, you accept that you are using it "as it is" and the creator of the engine is not responsible any damages, losses or any other kind of harm (financial or other) that is coming from using the engine. The user this engines takes all the responsibilities with it.
