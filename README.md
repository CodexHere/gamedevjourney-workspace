## Project Setup

First, we need to set up VSCode with Unity: https://code.visualstudio.com/docs/other/unity
* Install dotnet sdk: https://dotnet.microsoft.com/download
* Install Unity: https://docs.unity3d.com/hub/manual/InstallHub.html#install-hub-linux
* Install mono: https://www.mono-project.com/download/stable/

### System Dependencies

If you [receive an error about SSL](https://forum.unity.com/threads/workaround-for-libssl-issue-on-ubuntu-22-04.1271405/), you'll need to ensure `libssl1.x` is on your system.

Simply [download](http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2.16_amd64.deb) the Debian package, or [browse the archive](http://archive.ubuntu.com/ubuntu/pool/main/o/openssl) for the latest version.

### Unity Packages

For Unity Jobs, you'll need to install the following packages:
* `com.unity.jobs`
* `com.unity.nuget.mono-cecil`
* `com.unity.mathematics`

## Jobs Operations:

1) Create Empty Vectors
    - On a thread, but all in the same thread
    - Initialize our vector spaces for later population
2) Generate Noise Per Vector
    - In parallel, processes each Vector in it's own thread instance
    - Uses Perlin Noise to populate noise per vertex index
3) Modify Vertices to rules given in the inspector, as it relates to the noise values
    - Scalar-values, or IsoSurface, will be calculated for later evaluation when Marching Cubes.
4) Generate the mesh data:
    - Vertice list
    - Triangle list, which should reuse vertices as much as possible
5) ???
6) RENDER!
