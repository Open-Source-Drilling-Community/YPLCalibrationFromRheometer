<!-- PROJECT SHIELDS -->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![WebSite][website-shield]][website-url]

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
[website-shield]: https://img.shields.io/badge/foo-bar-black.svg?style=flat-square
[website-url]: https://opensourcedrilling.org/

Yield Power Law Model Calibration Methods
===

Content
===
This repository contains a microservice and webapp used to calibrate flow curves against the Yield Power Law rheological behavior (Herschel-Bulkley) and correct Fann35 rheometer measurements for non-Newtonian effects after Skadsem and Saasen (2019) and Lac and Parry (2017).

Scope
===
Drilling and completion operations are strongly affected by the rheological behavior of the mud and the cement slurry. For instance, it directly impacts the cleaning of the hole from drilling debris, the stability of the well when pumps are off during a connection, the pump pressure and hence the global energy required to drill the well or the quality and setting time of cementing operations.

The drilling mud is formulated by the mud engineer who regularly checks for its viscosity which should not be too high to limit pressure losses and for its yield point which should not be too low to efficiently lift drilling cuttings. Whereas in the field a simple 2-parameters model such as the Bingham Plastic model is generally used to describe the rheological behavior of the mud, it is more and more widely recognized that drilling fluids are better described by 3-parameters models such as the Yield-Power-Law model, also known as Herschel-Bulkley model.

Every 6-12 hours, the mud engineer takes a mud sample from the mud pit and conduct rheometer measurements, using a concentric-cylinders-type rheometer, also known as Couette rheometer. In the field, the Fann35 R1B1 model is used as a standard. The measurement consists in: setting the fluid sample between a static inner cylinder and a rotating outer cylinder; varying the RPM of the outer cylinder in the range [3,6,100,200,300,600 RPM]; measuring the reactive torque of the inner cylinder. Under the assumption that the fluid behaves as Newtonian these values are then respectively converted into shear rate and shear stress values. The resulting relationship between the shear rate and the shear stress describes the intrinsic rheological behavior of the mud and is called the flow curve.

Adjusting the flow curve on the Yield-Power-Law curve gives more accurate results than adjusting it on a Bingham Plastic curve: this allows to better discriminate between different mud formulations and also to better describe the rheological behavior of the drilling mud at low shear rate, which is critical in the annulus where the shear rate of the drilling fluid drops significantly in comparison to the shear rates reached within the drill pipes.


Software features
===
The first objective of the program is to allow the user to calibrate (e.g. adjust/fit) any flow curve (obtained from any type of rheometer) against the Herschel-Bulkley model either by setting rheometer measurements manually or by loading csv/xlsx files that contain flow curves.

The second objective is to account for recent advancements reached in the field mud rheology that have shown that calibration performed on flow curves that have been built under the Newtonian assumption leads to inaccurate results for a wide range of drilling fluids (mud, spacer, slurry). Hence, based on Skadsem and Saasen (2019), the program allows the user to generate corrected rheometer measurements that account for non-Newtonian effects in the estimation of the shear rate, under the Yield-Power-Law assumption. Similarly, based on Lac and Parry (2017), an additional correction process is provided that accounts for non-Newtonian effects in the estimation of the shear stress when using 3 standard Fann35 rheometers (of type R1B1, R1B2 and R1B5).


Software architecture
===
The program is composed of: 
-	a webapp that allows to handle rheometer measurements and launch computations in a user-friendly way. It is packaged as a docker container and deployed on a NORCE server at the following endpoints (3 different webpages):
o	https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/Rheograms
o	https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCalibrations
o	https://app.digiwells.no/YPLCalibrationFromRheometer/webapp/YPLCorrections
-	a microservice that performs the core computations and can be queried by any 3rd party client (a web navigator or any other online application) through http-requests. It is packaged as a docker container and deployed on a NORCE server at the following endpoints:
o	https://app.digiwells.no/YPLCalibrationFromRheometer/api/Rheograms 
o	https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCalibrations
o	https://app.digiwells.no/YPLCalibrationFromRheometer/api/YPLCorrections
-	Its api is accessible at the following url:
o	https://app.digiwells.no/YPLCalibrationFromRheometer/api/swagger/index.html

More info
===
Both the microservice and the webapp, their API, architecture and workflow are described in the wiki page of the current github repository.

