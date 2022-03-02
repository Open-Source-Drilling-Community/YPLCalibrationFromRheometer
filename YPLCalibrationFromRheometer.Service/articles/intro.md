---
title: "How to use the field microservice?"
output: html_document
---

Typical Usage
===
1. Upload a new field using the `Post` web api method.
2. Call the `Get` method with the identifier of the uploaded field as argument. 
The return Json object contains the field description.
3. Optionally send a `Delete` request with the identifier of the field in order to delete the field if you do not 
want to keep the field uploaded on the microservice.


