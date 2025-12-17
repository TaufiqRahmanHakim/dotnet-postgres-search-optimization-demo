namespace CustomerDashboard.DTO {
    public class CustomerDto {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; 
    }
}
