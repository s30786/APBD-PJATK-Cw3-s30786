using System;
using System.Collections.Generic;
using System.Linq;
using APBD_Cw3_S30786.Data;
using APBD_Cw3_S30786.Models;
using APBD_Cw3_S30786.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Cw3_S30786.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<RoomDto>> GetAll(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool activeOnly = false)
    {
        lock (AppData.SyncRoot)
        {
            IEnumerable<Room> query = AppData.Rooms;

            if (minCapacity.HasValue)
                query = query.Where(r => r.Capacity >= minCapacity.Value);

            if (hasProjector.HasValue)
                query = query.Where(r => r.HasProjector == hasProjector.Value);

            if (activeOnly)
                query = query.Where(r => r.IsActive);

            var result = query.Select(MapToRoomDto).ToList();
            return Ok(result);
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<RoomDto> GetById([FromRoute] int id)
    {
        lock (AppData.SyncRoot)
        {
            var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);
            if (room is null) return NotFound();

            return Ok(MapToRoomDto(room));
        }
    }

    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<RoomDto>> GetByBuilding([FromRoute] string buildingCode)
    {
        lock (AppData.SyncRoot)
        {
            var rooms = AppData.Rooms
                .Where(r => string.Equals(r.BuildingCode, buildingCode, StringComparison.OrdinalIgnoreCase))
                .Select(MapToRoomDto)
                .ToList();

            return Ok(rooms);
        }
    }

    [HttpPost]
    public ActionResult<RoomDto> Create([FromBody] CreateRoomDto dto)
    {
        var room = new Room
        {
            Id = AppData.NextRoomId(),
            Name = dto.Name,
            BuildingCode = dto.BuildingCode,
            Floor = dto.Floor,
            Capacity = dto.Capacity,
            HasProjector = dto.HasProjector,
            IsActive = dto.IsActive
        };

        lock (AppData.SyncRoot)
        {
            AppData.Rooms.Add(room);
        }

        var result = MapToRoomDto(room);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, result);
    }

    [HttpPut("{id:int}")]
    public ActionResult<RoomDto> Update([FromRoute] int id, [FromBody] UpdateRoomDto dto)
    {
        lock (AppData.SyncRoot)
        {
            var existing = AppData.Rooms.FirstOrDefault(r => r.Id == id);
            if (existing is null) return NotFound();

            existing.Name = dto.Name;
            existing.BuildingCode = dto.BuildingCode;
            existing.Floor = dto.Floor;
            existing.Capacity = dto.Capacity;
            existing.HasProjector = dto.HasProjector;
            existing.IsActive = dto.IsActive;

            return Ok(MapToRoomDto(existing));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        lock (AppData.SyncRoot)
        {
            var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);
            if (room is null) return NotFound();

            var hasRelatedReservations = AppData.Reservations.Any(r => r.RoomId == id);
            if (hasRelatedReservations)
            {
                return Conflict(new ErrorResponse(
                    "ROOM_HAS_RESERVATIONS",
                    "Cannot delete room with related reservations"));
            }

            AppData.Rooms.Remove(room);
            return NoContent();
        }
    }

    private static RoomDto MapToRoomDto(Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        BuildingCode = room.BuildingCode,
        Floor = room.Floor,
        Capacity = room.Capacity,
        HasProjector = room.HasProjector,
        IsActive = room.IsActive
    };
}