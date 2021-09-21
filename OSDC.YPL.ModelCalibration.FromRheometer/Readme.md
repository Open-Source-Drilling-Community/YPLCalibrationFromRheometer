Yield Power Law Model Calibration from Steady State Rheometer Measurements
===

Objective
===
Provide a microservice that allows to upload rheograms and to calibrate them with either the method 
from Zamora/Kelissidis (https://doi.org/10.1016/j.petrol.2006.06.004) or the method from Mullineux (https://doi.org/10.1016/j.apm.2007.09.010).

Microservice Solution Organization
===
The Rheogram and Yield Power Law models are defined in **OSDC.YPL.ModelCalibration.FromRheometer.Model**.
There is an associated project that performs unit testing the calibration methods in **OSDC.YPL.ModelCalibration.FromRheometer.NUnit**.
The microservice itself is inside the project **OSDC.YPL.ModelCalibration.FromRheometer.Service**.
There is also a web client application that shows how to access the microservice in the project **OSDC.YPL.ModelCalibration.FromRheometer.Test**.
This web client application utilizes Json classes that are generated from json description files. The Json description files are generated from the C# 
classes belonging to the model project using a small command application defined in **OSDC.YPL.ModelCalibration.FromRheometer.JsonSD** and another small 
command application that generate the C# classes from a Json description file. The command application is defined in the project 
**OSDC.YPL.ModelCalibration.FromRheometer.JsonCL**. Those two command applications are called as pre-built calls of the test application defined in 
**OSDC.YPL.ModelCalibration.FromRheometer.Test**.

Microservice Architecture
====
The project **OSDC.YPL.ModelCalibration.FromRheometer.Service** is defined as a web API application. It implements the CRUD (Create, Read, Updated, Delete) API. 
An instance of the microservice runs here: https://app.DigiWells.no/YPLCalibrationFromRheometer
The URL of the controller is  `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values`.

Get
---
By sending a http get command to `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values`, we retrieve a Json array of the current IDs uploaded on the microservice.

To retrieve the details and the calibration of the yield power law model on the associated rheogram, we send a http get command to `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values/5`. 
With no additional option, the calibration is made with the Mullineux's method.

However, it is also possible to pass an option. If the option is `Zamora`, the method from Zamora/Kelessidis is used. For any other option, it the method from 
Mullineux that is used. Here is an example `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values/5/Zamora`.

With or without options, the result is a Json object that contains the parameters of the calibrated yield power law model and the associated rheogram.

Post
---
By sending a http post command to `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values`, it is possible to add a new rheogram. The rheogram data are described in a Json string.

Put
---
A http put command to a url containing the ID of a rheogram, e.g., `https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values/5`, results in either updating the rheogram if it existing 
from before or to add it if it did not existed. The rheogram data are described in a Json string.

Delete
---
It is possible to delete a rheogram if you know its ID. For example to delete the rheogram having the ID 5, one send a http delete command to the following URL:  
`https://app.DigiWells.no/YPLCalibrationFromRheometer/api/values/5`

Jscon data Schemas
----
The reference Json data schemas can be found in `https://app.DigiWells.no/YPLCalibrationFromRheometer/json-schemas`. One of the schema corresponds to 
the rheogram and is called **Rheogram.txt** and the schema for the yield power law model is contained in **YPLModel.txt**. Note that the Json schema of 
**YPLModel.txt** contains the description of **Rheogram.txt**.

Containerization
---
The project is containerized using docker. When the docker image is started, there are 6 predefined rheograms:
* A Newtonian fluid (ID 1)
* A power law fluid (ID 2)
* A Bingham plastic fluid (ID 3)
* A Herschel-Bulkley fluid (ID 4)
* A Quemada fluid (ID 5)
* Real measurements maded with an Anton Paar rheometer of a KCl/polymer fluid (ID 6)

Simple Web Application
===
It is also possible to access the microservice through a simple web application: `https://app.DigiWells.no/YPLCalibrationFromRheometer/Rheograms`. 
The simple web application allows to:
* list the currently uploaded rheograms on the microservice
* create new rheograms
* delete existing rheograms
* edit an existing rheogram
* view the details of an existing rheogram. In this web page, one can find the results of the fitting of the Yield Power Law rheological behavior to the rheogram
using the Zamora/Kelessidis method and the Mullineux method. The chi-square of the fitted results is also shown. A graph displays the original measurements and 
the estimated flow-curves based on both calibation methods.

Container
===
Docker images are regularly maintained. They are available under the following tag name: digiwells/osdcyplmodelcalibrationfromrheometerservice
