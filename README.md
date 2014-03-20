ODataVersioning - Proof of Concept
===============

Naive approach to handling OData Endpoint versioning using an HTTP header


In this example application using the `x-api-version` request header will determine which *Product* to use. For example:

```
GET /srv/Products HTTP/1.1
x-api-version: 1
```
Will return a collection of `SimpleProduct` entities that have the following properties:
```json
{
  "Id": 1,
  "Name": "A Product Name",
  "Price": 1.0
}
```
while
```
GET /srv/Products HTTP/1.1
x-api-version: 3
```
and
```
GET /srv/Products HTTP/1.1
```
Will return a collection of `CategorizedProduct` entities that have the following properties:
```json
{
  "Id": 1,
  "Name": "A Product Name",
  "Price": 1.0,
  "Category": "The worst category ever."
}
```

This implementation has the added benefit that if a `x-api-version` value is outside the range of supported versions, it will return a `406 Not Acceptable` error. Additionally if the `x-api-version` request header is not present, the implementation will assume the current version is desired.

In this example, the valid version range is 1 through 5.
