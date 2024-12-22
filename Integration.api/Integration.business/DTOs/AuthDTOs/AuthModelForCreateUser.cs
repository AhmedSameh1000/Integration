namespace AutoRepairPro.Business.DTO.AuthDTOs
{
    public class AuthModelForCreateUser
    {
        public string? Message { get; set; }
        public bool duplicateEmail { get; set; }
        public bool duplicatePhoneNumber { get; set; }
        public bool IsCreated { get; set; }
    }
}