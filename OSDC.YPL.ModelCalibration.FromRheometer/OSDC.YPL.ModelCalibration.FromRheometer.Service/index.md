---
title: "Project: Microservice to support calibration of Yield Power Law models using steady state Couette rheometer measurements"
output: html_document
---

Objective
===
This project contains the microservice that allows to access the management of calibration of Yield Power Law models using steady state Couette rheometer measurements.


Principles
===
The microservice provides a web API and is containerized using Docker. The web api endpoint is `YPLCalibrationFromRheometer/api/values`. It is possible to upload new rheograms 
(`Post` method), to modify already uploaded rheograms (`Put` method), to delete uploaded rheograms (`Delete` method). By calling the `Get` without arguments, 
it is possible to obtain a list of all the identifiers of the rheograms that have been uploaded. It is also possible 
to call the `Get`method with the ID of an uploaded rheogram. In that case, a calibrated yield power law model is returned.
Without additional argument, the Mullineux' method is used to calibrate the rheological behavior. With an argument, it is 
possible to choose between the Zamora/Kelessidis method (using `Zamora` as argument) or anything else for Mullineux's method.


