# Upload Rheogram Program
This program is used to upload a set of rheograms on the YPL Calibration From Rheometer Service. The program reads
a file called "RheogramSet.txt" and upload the defined rheograms.

The program can be run with one argument, i.e., the host name on which the program will connect.
If no argument is provided then it will use the default host, which is specified at the start of the program, 
i.e., app.DigiWells.no.

The file structure is the following:
```
Name'tab'Description'tab'RheometerType
ShearRate'tab'ShearStress
...
'blank line'
Name'tab'Description'tab'RheometerType
ShearRate'tab'ShearStress
...
```
The possible Rheometer types are:
- 1: Anton Paar with standard bob configuration
- 2: Model 35 R1B1
- 3: Model 35 R1B2

The program searches for a rheometer that matches these specificationd and that does not have any default shear rates.
If it cannot find one, it creates it.

The ShearRate shall be given in "reciprocal second". The ShearStress shall be given in Pascal.

A "blank line" defines the end of the measurements of the current rheogram.

By default, the Skadsem & Saasen shear rate correction and the Lac & Parry shear stress corrections are applied.


