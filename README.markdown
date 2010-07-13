GenesisEngine
======================================================================

# License

Licensed under the [Ms-PL](http://www.microsoft.com/opensource/licenses.mspx#Ms-PL).

# What is it?

GenesisEngine allows you to create and explore procedurally-generated planetary systems, from ground level all the way out into space.  At the moment there's no larger purpose other than to wander around in a virtual world.  It's written in C# with the XNA framework.

Mostly, this is a personal project that I'm using to experiment with new software development techniques and maybe learn something about procedural terrain generation and rendering along the way.  This project is NOT intended to be some kind of best-practices reference for, well, anything.  Some stuff is well-written and some stuff is terrible.  Over time I hope to improve the terrible stuff and add more good stuff.  I'd be happy to receive constructive feedback.

If you're looking to learn something about procedurally-generated terrain or rendering techniques, don't start here.  Really.  I don't have any original ideas - I'm just borrowing everything I know from people much smarter than me and re-implementing it badly.

This project is also not a good reference for high-performance game engines.  I'm explicitly choosing to favor clarity of code design over high performance when those two principles are in conflict.  My computer is reasonably fast and hardware is cheap - so there!

I'm going to [blog](http://blogs.msdn.com/elee) occasionally about the journey so feel free to follow along.

# How do I build and run it?

The GenesisEngine project is a Visual Studio 2010 solution with dependencies on StructureMap, Machine.Specifications, and Rhino Mocks (all included) and Microsoft's [XNA Game Studio 4.0](http://creators.xna.com/en-US/downloads) (not included).  Right now the project is Windows-only and won't run on the Xbox or Zune.  There's no build script or anything; just load the solution in Visual Studio, build it, and run.  If you run into any problems, let me know.

# What are the controls?

* W/A/S/D for left/right/forward/backward
* E/C for up/down
* Z to go to ground level
* -/= to decrease/increase camera speed
* ,/. to decrease/increase camera zoom level
* hold the right mouse button for mouselook
* U to toggle state updates (useful for inspecting the current quad node tree from different angles.)
* F to toggle wireframe rendering

Right now the camera is fixed to a universal reference frame, not a planetary reference frame, so if you want to explore close to the ground it's probably better to stay close to the north pole where camera-up matches planet-up.

# What's the state of the project?

I've got a decent start on basic terrain generation.  I can zoom all the way from ground level to high orbit and the terrain is mildly interesting.  There's obviously a ton of features left to implement.  Some things on the list include:

* Generating terrain patches on the GPU which will be much faster
* Texturing
* Normal maps
* Atmospheric effects
* Stars
* A better camera system that tracks the curve of the planet
* Interactive setting of options

# What if I want to learn about generating and rendering planets?

Here's a non-exhaustive list of links to get your started if you want to find people who know what they're doing:

* [Interactive Visualization of a Planetary System](http://i31www.ira.uka.de/publikationen/files/4_Studproject_JWinzen.pdf)
* [A Real-Time Procedural Universe](http://www.sponeil.net/)
* [Infinity: The Quest For Earth](http://www.infinity-universe.com)
* [Outerra](http://outerra.blogspot.com/)
* [Britonia](http://britonia-game.com/)
* [Virtual Terrain Project](http://vterrain.org/)
