# What is it?
[Alabaster](https://en.wikipedia.org/wiki/Alabaster) is a VR app enabling you to sculpt 3D models in virtual reality. It gives you a set of tools to intuitively transform your imagination into a virtual object (and with a 3D printer - into a physical one). Application supports Oculus and SteamVR.
# How it works?
Alabaster's modeling space is a [Signed Distance Function](https://en.wikipedia.org/wiki/Signed_distance_function) (SDF in short). User can modify underlying SDF with Tools, which use [GPGPU](https://en.wikipedia.org/wiki/General-purpose_computing_on_graphics_processing_units) to process and transform SDF. Modified SDF is then transformed into mesh with [Marching Cubes Algorithm](https://en.wikipedia.org/wiki/Marching_cubes) and rednered to screen.

# Technology behind Alabaster
Alabaster is powered by [Unity](https://unity.com/). It uses [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.9/manual/index.html) for handling VR sets.
# Example features

## Adding and removing material
![](Videos/material-tool-add.mp4)
![](Videos/material-tool-remove.mp4)
![](Videos/material-tool-color.mp4)

## Streching and denting the material
![](Videos/move-tool.mp4)

## Smoothing the material
![](Videos/smooth-tool.mp4)

## Painting the material
![](Videos/paint-tool.mp4)

## Pottery wheel
![](Videos/pottery-wheel.mp4)

## Dynamic scene lighting
![](Videos/scene-light.mp4)

## Modifable surface rendering parameters
![](Videos/render-types.mp4)

## Resizing/moving
![](Videos/resizing-moving.mp4)

## Undo/Redo
![](Videos/undo-redo.mp4)

## Layers
![](Videos/layers.mp4)

## Importing/Exporting .obj files
![](Videos/import.mp4)
