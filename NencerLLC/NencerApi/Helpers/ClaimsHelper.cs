using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;

namespace NencerApi.Helpers
{
    public class ClaimsHelper
    {
        // Phương thức kiểm tra xem RoomIds trong claim có chứa id cụ thể không
        public static bool RoomIdExists(ClaimsPrincipal user, int? roomId = null)
        {
            // Lấy giá trị RoomIds từ claims
            var roomIdsString = user.Claims.FirstOrDefault(s => s.Type == "RoomIds")?.Value;

            // Kiểm tra xem RoomIds có giá trị không
            if (string.IsNullOrEmpty(roomIdsString))
            {
                return false;
            }

            roomIdsString = roomIdsString.ToLower();
            if (roomIdsString.Contains("all"))
            {
                return true;
            }

            //trường hợp get all phòng
            if (roomId.GetValueOrDefault() == 0)
            {
                if (!roomIdsString.Contains("all"))
                {
                    return false;
                }
            }

            // Tách chuỗi RoomIds thành danh sách các id
            var roomIds = roomIdsString.Split(',').Select(int.Parse).ToList();
            // Kiểm tra xem danh sách có chứa id cần tìm không
            return roomIds.Contains(roomId.Value);
        }

        public static string GetUsername(ClaimsPrincipal user)
        {
            var userName = user.Claims.FirstOrDefault(s => s.Type == "Username")?.Value;
            return userName;
        }

        public static int GetUserId(ClaimsPrincipal user)
        {
            int.TryParse(user.Claims.FirstOrDefault(s => s.Type == "Id")?.Value, out int UserId);
            return UserId;
        }
    }
}
