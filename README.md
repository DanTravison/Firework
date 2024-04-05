# Firework Animation Experiment

***NOTE: Currently only tested on Windows 11 and a 3rd Generaton IPad Pro 17.4.1.***

## Overview 
I'm creating on a simple maze program for the younger kids in my extended family.
It presents hand-drawn mazes with a start and end location and the touch is used
to draw a line from the start to end. When they solve the maze, I want to present a fun animation
to show the maze has been solved.

## Goals
* The solution is reasonably self-contained without adding additional, external dependencies.
* SkiaSharp based - SkiaSharp currently satisfies all my needs for image manipulation, drawing, 
and user interaction and want avoid introducing yet another graphics API.
* I'm not an artist so character animations are a non-starter.

After investigating numerous approaches, I decided on a simple fireworks animation. 
Frankly, there are some impressive simulations of fireworks on the web but they 
are either commercial, too expensive to port to SkiaSharp, or rely too heavily 
dependent on an existing game engine, such as Unity.

I narrowed my focus to c#-based solutions that native APIs and ended up deciding to port 
https://github.com/rehwinkel/FireworkScreenSaver to .Net Maui using SkiaSharp.

## High-level changes:

* The static FireworkRenderer was rewritten as FireworkAnimation, placing
the Particle list as an instance variable.

* The Particle-specific static variables and constants are now defined in Particle.cs

* Spark creation logic has been moved to the static Spark.AddSparks method for better encapsulation

* Hack: Simplified the firework's speed as it ascends and throttle the height to 
  avoid ascending above the top of the canvas. 
  * The original version allowed this to occur with larger canvas heights.

* Add support for pausing the animation.

* Restart the animation if the canvas's size changes.

* Various minor tweaks to color selection.

* Provide a UI for viewing the animation with controls to Start/Pause/Stop the 
animation and adjust the framerate.

* Define a variable to control how often a firework is launched.

## FrameworkAnimation Public APIs:

* Constructor - constructs the animation.
  * Accepts the SKCanvasView to draw to and the delay between launching fireworks.

* Speed Property - sets the animation's frame rate (frames per second)

* FireworkAnimation.Start - starts or resumes the animation

* FireworkAnimation.Pause - pauses the animation.

* FireworkAnimation.Stop - stops the animation.

## How it functions

* FireworkAnimation launches a firework particle based on the delay value passed 
to the constructor. The Firework moves up the screen until an apogee is 
encounter then creates Spark instances to simulate the explosion of the firework.

* Sparks spread and fall, fading in color until their lifetime has been reached.

## Todo

1: Investigate a better method for controlling the velocity of a firework as it ascends.
* Support variable velocities. 
* Decelerate as it approaches apogee to avoid sudden stops at apogee.
* Ensure velocity calculations are independent of frame rate.

2: Investigate methods for Sparks to provide variable spreading ranges. 

3: Randomize the number of sparks for ball and spark instances.

4: Randomize the lifetime of individual sparks.

5: Define a UI control to determine the firework launch delay.
