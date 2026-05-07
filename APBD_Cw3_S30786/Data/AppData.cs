using System;                           //nie wiem czemu ale jednego dnia wszystko było fajnie a następnego muszę takie coś dodawać
using System.Collections.Generic;
using System.Linq;
using APBD_Cw3_S30786.Models;

namespace APBD_Cw3_S30786.Data;

public static class AppData
{
    public static List<Room> Rooms { get; } = new();
    public static List<Reservation> Reservations { get; } = new();

    public static object SyncRoot { get; } = new();

    private static bool _seeded;

    public static void Seed()
    {
        lock (SyncRoot)
        {
            if (_seeded) return;

            Rooms.AddRange(new[]
            {
                new Room { Id = 1, Name = "Aula", BuildingCode = "A", Floor = 1, Capacity = 64, HasProjector = true, IsActive = true },
                new Room { Id = 2, Name = "Lab", BuildingCode = "B", Floor = 2, Capacity = 32, HasProjector = true, IsActive = true },
                new Room { Id = 3, Name = "Room", BuildingCode = "A", Floor = 3, Capacity = 12, HasProjector = false, IsActive = true },
                new Room { Id = 4, Name = "pomocy", BuildingCode = "C", Floor = 4, Capacity = 67, HasProjector = false, IsActive = false },
                new Room { Id = 5, Name = "Hall", BuildingCode = "B", Floor = 0, Capacity = 100, HasProjector = false, IsActive = true }
            });

            Reservations.AddRange(new[]
            {
                new Reservation
                {
                    Id = 1, RoomId = 1, OrganizerName = "Piotr Nowak",
                    Topic = "REST Basics", Date = new DateOnly(2026, 5, 10),
                    StartTime = new TimeOnly(9, 0, 0), EndTime = new TimeOnly(11, 0, 0),
                    Status = "confirmed"
                },
                new Reservation
                {
                    Id = 2, RoomId = 2, OrganizerName = "Michał Kurdefela",
                    Topic = "ASP.NET Core Workshop", Date = new DateOnly(2026, 5, 10),
                    StartTime = new TimeOnly(10, 0, 0), EndTime = new TimeOnly(12, 30, 0),
                    Status = "planned"
                },
                new Reservation
                {
                    Id = 3, RoomId = 3, OrganizerName = "Romek Wiskowski",
                    Topic = "HTTP Consultation", Date = new DateOnly(2026, 5, 11),
                    StartTime = new TimeOnly(8, 30, 0), EndTime = new TimeOnly(9, 30, 0),
                    Status = "confirmed"
                },
                new Reservation
                {
                    Id = 4, RoomId = 1, OrganizerName = "Piotr Łotr",
                    Topic = "API Review", Date = new DateOnly(2026, 5, 11),
                    StartTime = new TimeOnly(13, 0, 0), EndTime = new TimeOnly(14, 0, 0),
                    Status = "cancelled"
                },
                new Reservation
                {
                    Id = 5, RoomId = 5, OrganizerName = "ktos inny",
                    Topic = "cos robi", Date = new DateOnly(2026, 5, 12),
                    StartTime = new TimeOnly(15, 0, 0), EndTime = new TimeOnly(17, 0, 0),
                    Status = "confirmed"
                }
            });

            _seeded = true;
        }
    }

    public static int NextRoomId()
    {
        lock (SyncRoot)
        {
            return Rooms.Count == 0 ? 1 : Rooms.Max(r => r.Id) + 1;
        }
    }

    public static int NextReservationId()
    {
        lock (SyncRoot)
        {
            return Reservations.Count == 0 ? 1 : Reservations.Max(r => r.Id) + 1;
        }
    }
}