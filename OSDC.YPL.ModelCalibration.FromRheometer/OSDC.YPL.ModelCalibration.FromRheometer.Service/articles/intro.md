---
title: "How to use the Yield Power Law calibration microservice?"
output: html_document
---

Typical Usage
===
1. Upload a new rheogram using the `Post` web api method.
2. Call the `Get` method with the identifier of the uploaded rheogram as argument. If the Zamora/Kelissidis method is desired, also pass an additional parameter `Zamora`.
The return Json object contains the calibrated yield power law model, the chi-square of the calibration and the associated rheogram.
3. Optionally send a `Delete` request with the identifier of the rheogram in order to delete the rheogram if you do not 
want to keep the rheogram uploaded on the microservice.


