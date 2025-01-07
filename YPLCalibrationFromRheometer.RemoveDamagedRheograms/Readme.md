# Program to remove unloadable rheograms
Sometime a rheogram may be corrupted on the YPLCalibrationFromRheometer.Server. This may cause problems in the
YPLCalibrationFromRheometer.WebApp.Client. This program can be used to find rheograms that cannot be dowloaded
and to remove them.

The program can be run with one argument, i.e., the host name on which the program will connect.
If no argument is provided then it will use the default host, which is specified at the start of the program, 
i.e., app.DigiWells.no.