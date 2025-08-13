namespace Employee_Crud.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public decimal? Salary { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? UploadPicture { get; set; }
        public int? PayrollType { get; set; }
    }

    public class ServiceResponseDto<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

    }
}
