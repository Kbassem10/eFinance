using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Application.DTOs;

namespace StudentRegistrationPortal.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/admin/lookups")]
[Authorize(Roles = "Admin")]
public class AdminLookupsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminLookupsController> _logger;

    public AdminLookupsController(IUnitOfWork unitOfWork, ILogger<AdminLookupsController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Departments
    [HttpGet("departments")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments([FromQuery] string departmentCode = "", CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Lookups.GetAllDepartmentsAsync(departmentCode, cancellationToken);
        return Ok(result);
    }

    [HttpGet("departments/{id:int}")]
    [ProducesResponseType(typeof(DepartmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDepartmentById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetDepartmentByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new { message = $"Department ID {id} not found." });
        return Ok(result);
    }

    [HttpPost("departments")]
    [ProducesResponseType(typeof(DepartmentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateUpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.DepartmentName) || string.IsNullOrWhiteSpace(dto.DepartmentCode))
        {
            return BadRequest(new { message = "Department code and name are required." });
        }

        int newId = await _unitOfWork.Lookups.CreateDepartmentAsync(dto, cancellationToken);
        var created = await _unitOfWork.Lookups.GetDepartmentByIdAsync(newId, cancellationToken);
        return CreatedAtAction(nameof(GetDepartmentById), new { id = newId }, created);
    }

    [HttpPut("departments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDepartment([FromRoute] int id, [FromBody] CreateUpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.DepartmentName) || string.IsNullOrWhiteSpace(dto.DepartmentCode))
        {
            return BadRequest(new { message = "Department code and name are required." });
        }

        bool updated = await _unitOfWork.Lookups.UpdateDepartmentAsync(id, dto, cancellationToken);
        if (!updated) return NotFound(new { message = $"Department ID {id} not found." });
        return Ok(new { message = "Department updated successfully." });
    }

    [HttpDelete("departments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDepartment([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _unitOfWork.Lookups.DeleteDepartmentAsync(id, cancellationToken);
            if (!deleted) return NotFound(new { message = $"Department ID {id} not found." });
            return Ok(new { message = "Department deleted successfully." });
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            return BadRequest(new { message = "Cannot delete department because it is assigned to existing students, instructors, or courses." });
        }
    }
    #endregion

    #region Semesters
    [HttpGet("semesters")]
    [ProducesResponseType(typeof(IReadOnlyList<SemesterDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesters(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetAllSemestersAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("semesters/{id:int}")]
    [ProducesResponseType(typeof(SemesterDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSemesterById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetSemesterByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new { message = $"Semester ID {id} not found." });
        return Ok(result);
    }

    [HttpPost("semesters")]
    [ProducesResponseType(typeof(SemesterDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSemester([FromBody] CreateUpdateSemesterDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.SemesterName) || string.IsNullOrWhiteSpace(dto.AcademicYear))
        {
            return BadRequest(new { message = "Semester name and academic year are required." });
        }

        int newId = await _unitOfWork.Lookups.CreateSemesterAsync(dto, cancellationToken);
        var created = await _unitOfWork.Lookups.GetSemesterByIdAsync(newId, cancellationToken);
        return CreatedAtAction(nameof(GetSemesterById), new { id = newId }, created);
    }

    [HttpPut("semesters/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSemester([FromRoute] int id, [FromBody] CreateUpdateSemesterDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.SemesterName) || string.IsNullOrWhiteSpace(dto.AcademicYear))
        {
            return BadRequest(new { message = "Semester name and academic year are required." });
        }

        bool updated = await _unitOfWork.Lookups.UpdateSemesterAsync(id, dto, cancellationToken);
        if (!updated) return NotFound(new { message = $"Semester ID {id} not found." });
        return Ok(new { message = "Semester updated successfully." });
    }

    [HttpDelete("semesters/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSemester([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _unitOfWork.Lookups.DeleteSemesterAsync(id, cancellationToken);
            if (!deleted) return NotFound(new { message = $"Semester ID {id} not found." });
            return Ok(new { message = "Semester deleted successfully." });
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            return BadRequest(new { message = "Cannot delete semester because active course offerings or enrollments exist for this semester." });
        }
    }
    #endregion

    #region Rooms
    [HttpGet("rooms")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetAllRoomsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("rooms/{id:int}")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Lookups.GetRoomByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new { message = $"Room ID {id} not found." });
        return Ok(result);
    }

    [HttpPost("rooms")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateUpdateRoomDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.BuildingName) || string.IsNullOrWhiteSpace(dto.RoomNumber) || dto.Capacity <= 0)
        {
            return BadRequest(new { message = "Building name, room number, and positive capacity are required." });
        }

        int newId = await _unitOfWork.Lookups.CreateRoomAsync(dto, cancellationToken);
        var created = await _unitOfWork.Lookups.GetRoomByIdAsync(newId, cancellationToken);
        return CreatedAtAction(nameof(GetRoomById), new { id = newId }, created);
    }

    [HttpPut("rooms/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoom([FromRoute] int id, [FromBody] CreateUpdateRoomDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.BuildingName) || string.IsNullOrWhiteSpace(dto.RoomNumber) || dto.Capacity <= 0)
        {
            return BadRequest(new { message = "Building name, room number, and positive capacity are required." });
        }

        bool updated = await _unitOfWork.Lookups.UpdateRoomAsync(id, dto, cancellationToken);
        if (!updated) return NotFound(new { message = $"Room ID {id} not found." });
        return Ok(new { message = "Room updated successfully." });
    }

    [HttpDelete("rooms/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoom([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _unitOfWork.Lookups.DeleteRoomAsync(id, cancellationToken);
            if (!deleted) return NotFound(new { message = $"Room ID {id} not found." });
            return Ok(new { message = "Room deleted successfully." });
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            return BadRequest(new { message = "Cannot delete room because it is assigned to scheduled classes or lectures." });
        }
    }
    #endregion

    #region Status Tables Generic Endpoints
    [HttpGet("status-tables/{tableName}")]
    [ProducesResponseType(typeof(IReadOnlyList<StatusLookupItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStatusItems([FromRoute] string tableName, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _unitOfWork.Lookups.GetStatusItemsAsync(tableName, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("status-tables/{tableName}/{id:int}")]
    [ProducesResponseType(typeof(StatusLookupItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusItemById([FromRoute] string tableName, [FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _unitOfWork.Lookups.GetStatusItemByIdAsync(tableName, id, cancellationToken);
            if (result == null) return NotFound(new { message = $"Item ID {id} in {tableName} not found." });
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("status-tables/{tableName}")]
    [ProducesResponseType(typeof(StatusLookupItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStatusItem([FromRoute] string tableName, [FromBody] CreateUpdateStatusItemDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.StatusName))
        {
            return BadRequest(new { message = "StatusName is required." });
        }

        try
        {
            int newId = await _unitOfWork.Lookups.CreateStatusItemAsync(tableName, dto, cancellationToken);
            var created = await _unitOfWork.Lookups.GetStatusItemByIdAsync(tableName, newId, cancellationToken);
            return CreatedAtAction(nameof(GetStatusItemById), new { tableName, id = newId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("status-tables/{tableName}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusItem([FromRoute] string tableName, [FromRoute] int id, [FromBody] CreateUpdateStatusItemDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.StatusName))
        {
            return BadRequest(new { message = "StatusName is required." });
        }

        try
        {
            bool updated = await _unitOfWork.Lookups.UpdateStatusItemAsync(tableName, id, dto, cancellationToken);
            if (!updated) return NotFound(new { message = $"Item ID {id} in {tableName} not found." });
            return Ok(new { message = "Status item updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("status-tables/{tableName}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStatusItem([FromRoute] string tableName, [FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _unitOfWork.Lookups.DeleteStatusItemAsync(tableName, id, cancellationToken);
            if (!deleted) return NotFound(new { message = $"Item ID {id} in {tableName} not found." });
            return Ok(new { message = "Status item deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            return BadRequest(new { message = $"Cannot delete item from {tableName} because it is referenced by existing database records." });
        }
    }
    #endregion
}
