using Integration.business.DTOs.ReferenceDTos;

namespace Integration.business.Services.Interfaces
{
    public interface IReferenceService
    {
        public Task<ApiResponse<bool>> AddReference(ReferenceForCreateDTO reference);
    }

}
