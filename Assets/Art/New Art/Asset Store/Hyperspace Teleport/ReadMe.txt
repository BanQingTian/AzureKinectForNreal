Thank you for downloading Hyperspace Teleport !
Let's make hyperspace teleport effect in unity3d engine.
=============================================================================================================================================================
How to setup ?
  => Create new material with shader "Hyperspace Teleport/Standard".
  => Apply this material to your gameobject.
  => Attach script "HyperspaceTravel" to this gameobject.
  => Create a plane as the "clip plane".
  => Attach the clip plane gameobject to script "HyperspaceTravel" member "Clip Plane".

Now you are ready. Play hyperspace travel effect as you wish !
Try tweak inspectors of "HyperspaceTravel" script, you will get variational effect:
  => Dir: the clip plane cut direction.
  => Clip Func: which side of clip plane you want to cut.
  => Halo Color: the color of clip edge halo.
  => Clip: clip distance.
  => Halo: halo edge distance.
  => Alpha: transparency of clip plane.

Version 1.4 add a new kind of teleport dissolve effect.
Select the behind group in hierarchy, there are 3 material parameters:
  => Dir: direction of dissolve high light.
  => Emission：color of dissolve high light.
  => Dissolve：progress of dissolve effect.
If you want to use it on your own model, just use shader "Hyperspace Teleport/Standard2" and tweak material parameters by your own script.

Version 1.6 add two new kinds of hyperspace teleport shaders.
  => "Hyperspace Teleport/Standard3"
  => "Hyperspace Teleport/Standard4"
  they both utility the power of geometry shader to represent fancy teleport effect.

Demo scene demonstrate all features. Please refer to it as example.
=============================================================================================================================================================
If you like it, please give it a good review on asset store. Thanks so much ! It means a lot to us as publisher.
Any suggestion or improvement you want, please contact qq_d_y@163.com.
Hope we can help more and more game developers.