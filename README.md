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
[stars-url]: https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/stargazers
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

-	a webapp that allows to handle rheometer measurements and launch computations in a user-friendly way. It is packaged as a [docker](https://www.docker.com/) container and deployed on a NORCE server at the following endpoints (3 different webpages):

    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/Rheograms
    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCalibrations
    - https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCorrections

-	a microservice that performs the core computations and can be queried by any 3rd party client (a web navigator or any other online application) through http-requests. It is packaged as a [docker](https://www.docker.com/) container and deployed on a NORCE server at the following endpoints:

    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/Rheograms 
    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCalibrations
    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCorrections

-	Based on [Swagger](https://swagger.io/), API documentation of the microservice is accessible at the following url:

    - https://app.digiwells.no/YPLCalibrationFromRheometer/api/swagger/index.html

# Software solution layout

The program is packaged as a Visual Studio solution, developed in C# and based on the .NET Core 3.1 framework (compatible with Windows, Linux and MacOS).

Visual Studio has been used for the developmenent but as any dotnet program, it is possible to build it:
- from the free version of Visual Studio, e.g. Community Edition (see [here](https://visualstudio.microsoft.com/vs/community/)), with the advantage that the `.sln` file at the root of the current directory is directly readable by VS Community
- from the command line (assuming dotnet is already installed, otherwise see [here](https://dotnet.microsoft.com/en-us/download))
- from the light-weight generic Visual Studio Code (see [here](https://code.visualstudio.com/))

The solution is structure as follows:

```bash
YPLCalibrationFromRheometer/
│
├── Model/
│   ├── data model & associated calculations
│   └── dotnet framework = netcoreapp3.1
├── Service/
│   ├── microservice API
│   └── data persistence
│   └── dotnet framework = netcoreapp3.1
├── WebApp.Client/
│   ├── simple dotnet blazor-server-based webapp
│   └── dotnet framework = net6.0 (plotting features based on Plotly.Blazor)
├── RheometerCorrectionApp/
│   ├── Windows form based app to check-for correction algorithms
│   └── dotnet framework = netcoreapp3.1
├── Test
│   ├── Functional test-suite of the microservice API
│   └── dotnet framework = netcoreapp3.1
├── NUnit
│   ├── Unit test-suite of the core algorithms of the microservice
│   └── dotnet framework = netcoreapp3.1
├── ModelClientShared
│   ├── POCO version of the data model
│   └── dotnet framework = netcoreapp3.1
├── JsonSD
│   ├── Automatically generates the json schema from the data model
│   └── dotnet framework = netcoreapp3.1
└── JsonCL
    ├── Automatically generates a POCO version of the data model from its json schema
    └── dotnet framework = netcoreapp3.1
```

# Current status

-	Model
    - calibration of any flow curve against the Herschel-Bulkley model
    - calibration methods available: Zamora/Kelessidis and Mullineux
    - shear stress correction
        - standard correction of Fann35 shear stress rheometer measurements for non-Newtonian end effects 
        - rheology dependent correction of Fann35 (R1B1, R1B2, R1B5 configurations) shear stress rheometer measurements for non-Newtonian end effects (after [Lac and Parry, 2017](http://sor.scitation.org/doi/10.1122/1.4986925))
    - shear rate correction
        - correction of Couette rheometer measurements for non-Newtonian effects (after [Skadsem and Saasen, 2019](https://www.degruyter.com/document/doi/10.1515/arh-2019-0001/html))

-	Software
    - microservice handling Rheogram's, YPLCorrection's, YPLCalibration's and Unit's through a CRUD API where data are exchanged as json strings (.NET Core 3.1)
    - blazer-server webapp allowing to interact with the microservice (.NET 6.0)
    - microservice and webapp packaged as docker containers and deployed on a publicly accessible NORCE server (see [above](#program-features))
    - [SQLite](https://sqlite.org/index.html) data storage capability embedded into the container itself

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

# Go to WIKI for more!

[WIKI](https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/wiki) page includes:
- [Background information on the model](https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/wiki/Background)
- [A tutorial on the use of the webapp](https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/wiki/WebApp-tutorial)
- [A tutorial on the use of the microservice](https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/wiki/Microservice-tutorial)
- [A tutorial on the software architecture and design choices](https://github.com/Open-Source-Drilling-Community/YPLCalibrationFromRheometer/wiki/Software-architecture)


