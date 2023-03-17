<!-- PROJECT SHIELDS -->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

[contributors-shield]: https://img.shields.io/github/contributors/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?logo=GitHub
[contributors-url]: https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?logo=GitHub
[forks-url]: https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/network/members
[stars-shield]: https://img.shields.io/github/stars/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?color=%230000ff&logo=GitHub
[stars-url]: https://img.shields.io/github/stars/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?style=flat-square
[issues-shield]: https://img.shields.io/github/issues/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?color=%23FF0000&logo=GitHub
[issues-url]: https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/issues
[license-shield]: https://img.shields.io/github/license/Open-Source-Drilling-Community/YPLCalibrationFromRheometer?color=%2300FFFF
[license-url]: https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/blob/master/LICENSE


Herschel-Bulkley Calibration and Correction of Fann35 Rheometer Measurements 
===

[![image](https://user-images.githubusercontent.com/91899139/225950172-311ea20f-c248-40cc-90a4-8d981e6b89f4.png)](https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/Rheograms)

# Table of content

- [About this repo](#about-this-repo)
- [Overview](#overview)
- [Program features](#program-features)
- [Software solution layout](#software-solution-layout)
- [Current status](#current-status)
- [License](#license)
- [Contributions](#contributions)
- [Contact](#contact)
- [Go to wiki for more!](#go-to-wiki-for-more)

# About this repo
This repository contains a program (microservice and webapp) used to calibrate flow curves against the [Herschel-Bulkley model](https://en.wikipedia.org/wiki/Herschel%E2%80%93Bulkley_fluid) and correct [Fann35 rheometer](https://www.fann.com/en/products/model-35) measurements for non-Newtonian effects.

# Overview
The program:

-	allows to calibrate (e.g. adjust/fit) any flow curve (obtained from any type of rheometer) against the Herschel-Bulkley model either by setting rheometer measurements manually or by loading csv/xlsx files that contain flow curves.

-	accounts for recent advancements in rheology that have shown that calibration performed on flow curves that have been built under the Newtonian assumption leads to inaccurate results for a wide range of drilling fluids (mud, spacer, slurry). 

    - based on the work of [Skadsem and Saasen (2019)](https://www.degruyter.com/document/doi/10.1515/arh-2019-0001/html), the program allows the user to generate corrected rheometer measurements that account for non-Newtonian effects in the estimation of the shear rate, under the Yield-Power-Law assumption.

    - similarly, based on [Lac and Parry (2017)](http://sor.scitation.org/doi/10.1122/1.4986925), a complementary rheology-dependent correction for non-Newtonian effects associated with Fann35 geometry end-effects is performed. Corresponding correction functions derived by Lac and Parry apply to 3 standard [Fann35 rheometer](https://www.fann.com/en/products/model-35) configurations (of type R1B1, R1B2 and R1B5). For other Fann35 configurations, a standard correction factor of 0.064 is applied to the shear stress measured.

# Program features
The program is composed of: 

-	a webapp that allows to handle rheometer measurements and launch computations in a user-friendly way. It is packaged as a docker container and deployed on a NORCE server at the following endpoints (3 different webpages):

    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/Rheograms
    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCalibrations
    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCorrections

-	a microservice that performs the core computations and can be queried by any 3rd party client (a web navigator or any other online application) through http-requests. It is packaged as a docker container and deployed on a NORCE server at the following endpoints:

    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/Rheograms 
    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCalibrations
    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCorrections

-	Based on [Swagger](https://swagger.io/), API documentation of the microservice is accessible at the following url:

    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/swagger/index.html

# Software solution layout

The development is made in C#. The projects utilize .NET Standard LTS (.NET Core 3.1) and therefore are compatible with both Windows, Linux and MacOS operating systems. Visual Studio is used for the developmenent but it should be possible to build the solution from the command line for those who just have the .Net environment without Visual Studio.

The software solutions contain:

- A model where the actual calibration and correction are made.
- Unit tests to control that the implemented method works and that any later modifications do not break backward compatibility.
- A microservice implemented and containerized with docker. The web API can be found in [host]/[service]/api/values where [service]=OSDC.YPL.ModelCalibration.FromRheometer and [host]=https://app.DigiWells.no
- An example test program to check that the container works properly.
- The web service also provides online documentation. The online documentation can be found in [service].
- The web service default web page provides a web interface to handle rheograms, calibrations and corrections according to the CRUD API.
- The web app client is based on Blazor server technology
- A SQLite database allows for data storage on the digiwells server but also locally if needed.

# Current status

-	Model

    - Calibration of YPL steady state from Couette rheometer measurements with the methods from Zamora/Kelessidis and Mullineux: OSDC.YPL.ModelCalibration.FromRheometer. Its docker image has the following tag: digiwells/osdcyplmodelcalibrationfromrheometerservice
  

-	Software

# License

This research was funded by the Norwegian Research Council, Equinor and Sekal as part of the project "6n Degrees of Freedom Transient Torque & Drag. The code is a donation made by NORCE and is distributed under the MIT license.

# Contributions

Contributors of the current repository are:
- original contributor: [Eric Cayeux](https://www.norceresearch.no/personer/eric-cayeux/526) ([NORCE](https://www.norceresearch.no/))
- shear rate YPL correction: [Gilles Pelfrene](https://www.norceresearch.no/personer/gilles-pelfrene/23354694) ([NORCE](https://www.norceresearch.no/)), [Benoît Daireaux](https://www.norceresearch.no/personer/benoit-daireaux/604) ([NORCE](https://www.norceresearch.no/))
- shear stress YPL correction: [Gilles Pelfrene](https://www.norceresearch.no/personer/gilles-pelfrene/23354694) ([NORCE](https://www.norceresearch.no/))


Contributions are what make the drilling open source community grow are greatly appreciated.
If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
1.	Fork the Project
2.	Create your Feature Branch (git checkout -b feature/your-feature-name)
3.	Commit your Changes (git commit -m 'Add new fancy feature')
4.	Push to the Branch (git push origin feature/your-feature-name)
5.	Open a Pull Request

# Contact

Eric Cayeux: erca@norceresearch.no

Gilles Pelfrene: gipe@norceresearch.no 

# Go to wiki for more!

Wiki page includes:
-	Model
    - Short introduction on the importance of drilling fluid rheometry
    - Description of calibration methods used
    - Description of correction methods used
    - Bibliography
-	Software
    - Installation process


