==========================

TWOD Framework

Version 0000.00001

- By David López Meseguer

==========================

A 2D framework to Animate graphics from the same .Png/.jpg file.
This framework answer the question of "Y U NO GIF UNITY?" by providing a way
to animate easily for Artists in Unity.

Main Functionality:
It crops the image into pieces and animate them along the X axis of the original image.
On the Y axis more Animations are stored:

<table> 
<tr><td colspan="6">Frames (x axis)</td><td> Animation name (y axis)</td></tr>
<tr><td>0</td><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td> "idle" </td></tr>
<tr><td>0</td><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td> "walking"</td> </tr>
<tr><td>0</td><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td> "attackin"</td> </tr>
</table>

Pure RPG-Maker style.
This makes it easy to use Software like PyxelEdit with Unity.

Features Animation System with built-in Events.
It supports any number of frames/animations and any size for "CellSize".

For example: Cell size = x: 50pixels y: 50pixels
will generate a 4 cell sliced Image from a 200x200 pixels image with 2 animations with 2 frames each.
On a 500x1000pixels image will generate a 10x20 sliced image with 20 animations, 10 frames each.

Current restriction:
The division of pixels has to be exact. A 201x200 pixels image on the 1st example above would fail.

![Alt text](https://www.dropbox.com/s/zxby0ys72zmid7q/twod1.gif?dl=1)

===========================
Progress:
===========================

Main Functionality complete.
Working on Custom Editor and Inspectors tools for easy access.
