Prosty schemat testów w postmanie:

GET    http://localhost:5062/api/rooms/1
GET    http://localhost:5062/api/rooms/999    (ERROR 404)

POST   http://localhost:5062/api/rooms

{
  "name": "APBD",
  "buildingCode": "C",
  "floor": 3,
  "capacity": 90,
  "hasProjector": true,
  "isActive": true
}

PUT    http://localhost:5062/api/rooms/2

{
  "name": "Lab – updated",
  "buildingCode": "B",
  "floor": 2,
  "capacity": 28,
  "hasProjector": true,
  "isActive": true
}

DELETE http://localhost:5062/api/rooms/2  (ROOM_HAS_RESERVATIONS)

DELETE http://localhost:5062/api/rooms/4  (DZIAŁA)
