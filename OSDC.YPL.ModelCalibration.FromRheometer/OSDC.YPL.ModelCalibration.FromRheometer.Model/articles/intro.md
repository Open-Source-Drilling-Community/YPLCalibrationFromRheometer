---
title: "How to use the Yield Power Law rheological model calibration with Couette rheometer steady state measurements?"
output: html_document
---

Synopsis
===
1. Create an instance of `Rheogram`.
2. Fill in the `Rheogram` with the measurements. Note that there shall be at least 3 measurements for calibrating the YPL model
3. Create an instance of `YPLModel`.
4. Call one of the two `FitTo` method with `Rheogram` as argument. The two possible `FitTo` methods are: `FitToKelessidis` or `FitToMullineux`. 
The `FitTo` methods calculate the yield stress, consistency index and flow behavior index. They also calculate the
chi-square of the model fitting. The chi-square depends on the standard deviation of rheometer measurements. If no standard
deviation has been specified, then the default is 0.01 Pa.

Differences between the two calibration methods
===
The `FitToKelessidis` method is very sensitive to the precision of the smallest values. Even using rheometer values that are
synthetically generated with 64bit precision, it will not be able to fit perfectly the YPL parameters if there are small
shear stresses.

The `FitToMullineux` method is robust to small shear stress values and their possible uncertainty. However, it does not 
work for shear thicknening fluids.
