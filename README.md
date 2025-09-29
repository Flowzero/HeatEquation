This project presents an implementation of the partial differential heat equation (PDE) for a two-dimensional space (in the simplest case, for a rod) using a graphical UI. This small project was implemented as my university assignment, maybe someone will need it too. 

### Brief description
Base.cs contains a base class for two solution methods presented in Shemes.cs. The user interface is located in the Form1.cs file. Basically, that's all you need.

### How does it work?
Base class contains fields you need to provide for initialization and a few methods:
* ```__init__grid()``` - initializes nPoints (quantity of dx steps from start position to end position) and nTimeSteps (quantity of time frames). The code automatically calculates the time step dt (frame) based on the stability condition for the direct scheme. One time frame contains temperature values for all x coordinates. 
* ```__init__temperature``` - initializes the initial temperature distribution (temperature[nPoints]). By default, it creates a rectangular pulse (heat source) with a width of dx at position x = 0.4 with a temperature of 100°C, while the other points have a temperature of 0°C. This array represents a layer which
  gets updated every frame and shows how temperature distributes through time (See an example below).
* ```SolveStep()``` - an abstract method that must be implemented in derived classes to perform a single step of solving the heat equation.
* ```Reset()``` - resets the simulation to its initial state.

![tempDis](https://upload.wikimedia.org/wikipedia/commons/d/d9/Fundamental_solution_to_the_heat_equation.gif)
[Image source](https://en.wikipedia.org/wiki/File:Fundamental_solution_to_the_heat_equation.gif#file)
