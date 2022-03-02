cd %1\YPLCalibrationFromRheometer.Model
(docfx build -o %1\YPLCalibrationFromRheometer.Service\wwwroot\Rheogram\ModelAPI) || (echo "docfx for model failed")
cd %1\YPLCalibrationFromRheometer.Service
(docfx build -o %1\YPLCalibrationFromRheometer.Service\wwwroot\Rheogram\ServiceAPI) || (echo "docfx for service failed")
