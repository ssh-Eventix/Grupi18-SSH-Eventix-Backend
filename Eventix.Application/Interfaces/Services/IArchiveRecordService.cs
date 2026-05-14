using Eventix.Application.DTOs.Archive;

namespace Eventix.Application.Interfaces.Services;

public interface IArchiveRecordService
{
    Task<List<ArchiveRecordResponseDTO>> GetAllAsync();
    Task<ArchiveRecordResponseDTO?> GetByIdAsync(Guid id);
    Task<List<ArchiveRecordResponseDTO>> GetByEntityAsync(string entityName);
    Task<List<ArchiveRecordResponseDTO>> GetByYearAsync(int year);
    Task<ArchiveRecordResponseDTO> CreateAsync(CreateArchiveRecordDTO dto);
    Task<bool> DeleteAsync(Guid id);
}