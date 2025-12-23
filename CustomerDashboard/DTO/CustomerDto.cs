using MessagePack;

namespace CustomerDashboard.DTO {
    [MessagePackObject]
    public class CustomerDto {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string FullName { get; set; } = string.Empty; 
        [Key(2)]
        public string Email { get; set; } = string.Empty;
        [Key(3)]
        public string City { get; set; } = string.Empty;
        [Key(4)]
        public string Status { get; set; } = string.Empty; 
    }
}
