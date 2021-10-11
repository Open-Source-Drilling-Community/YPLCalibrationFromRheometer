Yield Power Law Model Calibration Methods
===

Objective
===
This repository contains micro-services to calibrate Yield Power Law rheological behaviors.

Scope
===
The calibration methods cover:
* calibration in steady state conditions
* calibration in transient conditions

The calibration methods are also available for different measurement types:
* a series of shear rate and shear stress for example measured with a couette rheometer
* a series of flow-rate and differential pressure for example with a pipe rheometer

Software solution layout
===
The software solutions contain:
* A model where the actual calibration is made.
* Unit tests to control that the implemented method works and that any later modifications do not break backward compatibility.
* A wrapper microservice implemented as a web api server and containerized with docker. The web API can be found in [service]api/values.
* An example test program to check that the container works properly.
* The web service also provides online documentation. The online documentation can be found in [service].
* The web service default page provide a simple web interface to access the functionalities of the web api.

The development is made in C#. The projects utilize .Net Standard and therefore are compatible with both Windows, Linux and MacOS operating systems. 
Visual Studio is used for the developmenent but it should be possible to build the solution from the command line for those who just have the .Net 
environment without Visual Studio.

Prerequisites
===
You need to have downloaded the .Net SDK environment and you need to have docker running on the machine where you will build the software solutions. 
The documentation is generated using DocFX.

Microservice Availability
===
The microservices are uploaded and running on a server hosted at NORCE. The server name is: https://app.DigiWells.no 

Containers
===
Container images are regularly uploaded on Docker. The utilized organization name is digiwells. 

Current status
===
At the moment, the following calibration solutions are available:
* Calibration of YPL steady state from Couette rheometer measurements with the methods from Zamora/Kelessidis and Mullineux: OSDC.YPL.ModelCalibration.FromRheometer. 
Its docker image has the following tag: digiwells/osdcyplmodelcalibrationfromrheometerservice

License
===
The code is a donation by NORCE and is made available under the MIT license.
The original contributors for the software solutions are:
* OSDC.YPL.ModelCalibration.FromRheometer: Eric Cayeux (NORCE)

