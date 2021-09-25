cd %1OSDC.YPL.ModelCalibration.FromRheometer.Model
(docfx build -o %1\OSDC.YPL.ModelCalibration.FromRheometer.Service\wwwroot\YPLCalibrationFromRheometer\ModelAPI) || (echo "docfx for model failed")
cd %1OSDC.YPL.ModelCalibration.FromRheometer.Service
(docfx build -o %1\OSDC.YPL.ModelCalibration.FromRheometer.Service\wwwroot\YPLCalibrationFromRheometer\ServiceAPI) || (echo "docfx for service failed")
