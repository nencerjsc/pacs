namespace NencerApi.Modules.User.Model
{
    public class AddUserInRooms_ReqDto
    {
        public int userId { get; set; }
        public List<int>? listRoomId { get; set; }
    }
}
