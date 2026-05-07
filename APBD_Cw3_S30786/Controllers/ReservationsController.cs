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
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<ReservationDto>> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        lock (AppData.SyncRoot)
        {
            IEnumerable<Reservation> query = AppData.Reservations;

            if (date.HasValue)
                query = query.Where(r => r.Date == date.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r =>
                    string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));

            if (roomId.HasValue)
                query = query.Where(r => r.RoomId == roomId.Value);

            var result = query.Select(MapToReservationDto).ToList();
            return Ok(result);
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<ReservationDto> GetById([FromRoute] int id)
    {
        lock (AppData.SyncRoot)
        {
            var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation is null) return NotFound();

            return Ok(MapToReservationDto(reservation));
        }
    }

    [HttpPost]
    public ActionResult<ReservationDto> Create([FromBody] CreateReservationDto dto)
    {
        lock (AppData.SyncRoot)
        {
            var room = AppData.Rooms.FirstOrDefault(r => r.Id == dto.RoomId);
            if (room is null)
            {
                return BadRequest(new ErrorResponse(
                    "ROOM_NOT_FOUND",
                    "Room does not exist."));
            }

            if (!room.IsActive)
            {
                return BadRequest(new ErrorResponse(
                    "ROOM_INACTIVE",
                    "Cannot create reservation for an inactive room"));
            }

            var candidate = MapToReservation(dto);
            candidate.Id = AppData.NextReservationId();

            if (HasTimeConflict(candidate))
            {
                return Conflict(new ErrorResponse(
                    "TIME_CONFLICT",
                    "Time conflict for the same room and date"));
            }

            AppData.Reservations.Add(candidate);

            var result = MapToReservationDto(candidate);
            return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, result);
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<ReservationDto> Update([FromRoute] int id, [FromBody] UpdateReservationDto dto)
    {
        lock (AppData.SyncRoot)
        {
            var existing = AppData.Reservations.FirstOrDefault(r => r.Id == id);
            if (existing is null) return NotFound();

            var room = AppData.Rooms.FirstOrDefault(r => r.Id == dto.RoomId);
            if (room is null)
            {
                return BadRequest(new ErrorResponse(
                    "ROOM_NOT_FOUND",
                    "Room does not exist"));
            }

            if (!room.IsActive)
            {
                return BadRequest(new ErrorResponse(
                    "ROOM_INACTIVE",
                    "Cannot create reservation for an inactive room"));
            }

            var candidate = MapToReservation(dto);
            candidate.Id = id;

            if (HasTimeConflict(candidate, id))
            {
                return Conflict(new ErrorResponse(
                    "TIME_CONFLICT",
                    "Time conflict for the same room and date"));
            }

            existing.RoomId = candidate.RoomId;
            existing.OrganizerName = candidate.OrganizerName;
            existing.Topic = candidate.Topic;
            existing.Date = candidate.Date;
            existing.StartTime = candidate.StartTime;
            existing.EndTime = candidate.EndTime;
            existing.Status = candidate.Status;

            return Ok(MapToReservationDto(existing));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        lock (AppData.SyncRoot)
        {
            var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation is null) return NotFound();

            AppData.Reservations.Remove(reservation);
            return NoContent();
        }
    }

    private static bool HasTimeConflict(Reservation candidate, int? excludedId = null)
    {
        return AppData.Reservations.Any(r =>
            r.Id != excludedId &&
            r.RoomId == candidate.RoomId &&
            r.Date == candidate.Date &&
            !string.Equals(r.Status, "cancelled", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(candidate.Status, "cancelled", StringComparison.OrdinalIgnoreCase) &&
            candidate.StartTime < r.EndTime &&
            candidate.EndTime > r.StartTime);
    }

    private static Reservation MapToReservation(CreateReservationDto dto) => new()
    {
        RoomId = dto.RoomId,
        OrganizerName = dto.OrganizerName,
        Topic = dto.Topic,
        Date = dto.Date,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        Status = dto.Status
    };

    private static ReservationDto MapToReservationDto(Reservation reservation) => new()
    {
        Id = reservation.Id,
        RoomId = reservation.RoomId,
        OrganizerName = reservation.OrganizerName,
        Topic = reservation.Topic,
        Date = reservation.Date,
        StartTime = reservation.StartTime,
        EndTime = reservation.EndTime,
        Status = reservation.Status
    };
}