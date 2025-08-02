namespace NencerApi.Modules.User.Model
{
    public class UserRoomDto
    {
        public int UserRoomId { get; set; }

        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public string? RoomNumber { get; set; }

        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserFullName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
