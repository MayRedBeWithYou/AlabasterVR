# What is it?
[Alabaster](https://en.wikipedia.org/wiki/Alabaster) is a VR app which lets you turn yourself into a sculptor. Import existing .obj model or start new project, create your 3D model using intuitive tools and export it to .obj.

# How it works?
Alabaster's modeling space is a [Signed Distance Function](https://en.wikipedia.org/wiki/Signed_distance_function) (SDF in short). User can modify underlying SDF with Tools, which use [GPGPU](https://en.wikipedia.org/wiki/General-purpose_computing_on_graphics_processing_units) to process and transform SDF. Modified SDF is then transformed into mesh with [Marching Cubes Algorithm](https://en.wikipedia.org/wiki/Marching_cubes) and rednered to screen.

# Technology behind Alabaster
Alabaster is powered by [Unity](https://unity.com/). It uses [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.9/manual/index.html) for handling VR sets.
