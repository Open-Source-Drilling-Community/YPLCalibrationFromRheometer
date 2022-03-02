---
title: "Project: Microservice for Rheograms"
output: html_document
---

Objective
===
This project contains the microservice that allows to access the management of Rheograms.


Principles
===
The microservice provides a web API and is containerized using Docker. The web api endpoint is `YPLCalibrationFromRheometer/api/Rheograms`. It is possible to upload new Rheogram 
(`Post` method), to modify already uploaded Rheogram (`Put` method), to delete uploaded Rheograms (`Delete` method). By calling the `Get` without arguments, 
it is possible to obtain a list of all the identifiers of the Rheograms that have been uploaded. It is also possible 
to call the `Get`method with the ID of an uploaded Rheogram.


