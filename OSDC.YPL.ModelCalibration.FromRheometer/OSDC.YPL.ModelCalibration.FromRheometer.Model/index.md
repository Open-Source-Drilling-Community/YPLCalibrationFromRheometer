---
title: "Project: Models for Yield Power Law calibration based on Couetter Rheometer measurements"
output: html_document
---

Objective
===
This project contains the models used to perform a calibration of a yield power law rheological behavior using Couette rheometer measurements.


Principles
===
Steady state Couette rheometer measurements are a measured shear stress for a given shear rate. 
A rheogram is a series of rheometer measurements taken in steady state conditions.
A steady state Yield Power Law rheological behavior is defined as: ![\tau =\tau_{0}+K \dot{\gamma}^n ](https://latex.codecogs.com/svg.latex?\tau =\tau_{0}+K \dot{\gamma}^n), 
where ![\tau](https://latex.codecogs.com/svg.latex?\tau) is the shear stress, ![\tau_{0}](https://latex.codecogs.com/svg.latex?\tau_{0}) is the yield stress,
![\dot{\gamma}](https://latex.codecogs.com/svg.latex?\dot{\gamma}) is the shear rate, 
![K](https://latex.codecogs.com/svg.latex?K) is the consistency index, and ![n](https://latex.codecogs.com/svg.latex?n) is the flow behavior index.

There are two implemented methods for calibrating the YPL rheological behavior:
* the method described by Zamora or Kelissidis (see https://doi.org/10.1016/j.petrol.2006.06.004).
* the method described by Mullineux (see https://doi.org/10.1016/j.apm.2007.09.010).



