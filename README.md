Yield Power Law Model Calibration Methods
===

Objective
===
This repository contains micro-services to calibrate Yield Power Law rheological behaviors and correct Couette rheometer measurements.

Scope
===
The calibration methods cover:
* calibration in steady state conditions
* calibration in transient conditions

The calibration methods are also available for different measurement types:
* a series of shear rate and shear stress for example measured with a Couette rheometer
* a series of flow-rate and differential pressure for example with a pipe rheometer

Shear rate and shear stress measurements obtained under the newtonian fluid assumption can be corrected for non-newtonian effects which should be the base scenario with standard drilling fluids.

Software solution layout
===
The software solutions contain:
* A model where the actual calibration and correction are made.
* Unit tests to control that the implemented method works and that any later modifications do not break backward compatibility.
* A microservice implemented and containerized with docker. The web API can be found in [host]/[service]/api/values where [service]=OSDC.YPL.ModelCalibration.FromRheometer and [host]=https://app.DigiWells.no
* An example test program to check that the container works properly.
* The web service also provides online documentation. The online documentation can be found in [service].
* The web service default web page provides a web interface to handle rheograms, calibrations and corrections according to the CRUD API.
* The web app client is based on Blazor server technology
* A SQLite database allows for data storage on the digiwells server but also locally if needed.

The development is made in C#. The projects utilize .NET Standard LTS (.NET Core 3.1) and therefore are compatible with both Windows, Linux and MacOS operating systems. 
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

