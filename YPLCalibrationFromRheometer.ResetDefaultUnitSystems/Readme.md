# Reset Default Unit Systems
This standalone program is used to restore the default unit system sets if any is missing.

The default unit system sets are:
- SI
- Metric
- US
- Imperial

Note that the version of the nuget OSDC.UnitConversion.DrillingEngineering in this application must match the version
used in YPLCalibrationFromRheometer.Service.

The program can be run with one argument, i.e., the host name on which the program will connect.
If no argument is provided then it will use the default host, which is specified at the start of the program, 
i.e., app.DigiWells.no.
