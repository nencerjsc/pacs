using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace NencerApi.Helpers
{
    public class WebSocketHandler
    {
        // Danh sách lưu trữ các kết nối WebSocket hiện tại
        private readonly List<WebSocket> _connections = new();

        // Phương thức xử lý kết nối WebSocket khi có client kết nối tới
        public async Task HandleAsync(HttpContext context)
        {
            // Kiểm tra xem yêu cầu hiện tại có phải là WebSocket request không
            if (context.WebSockets.IsWebSocketRequest)
            {
                // Chấp nhận yêu cầu WebSocket và lấy WebSocket object
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                // Thêm kết nối vào danh sách các kết nối đang quản lý
                _connections.Add(webSocket);

                // Lắng nghe tin nhắn từ WebSocket này
                await ListenAsync(webSocket);
            }
            else
            {
                // Nếu yêu cầu không phải là WebSocket, trả về mã lỗi 400
                context.Response.StatusCode = 400;
            }
        }

        // Phương thức lắng nghe tin nhắn từ một WebSocket cụ thể
        private async Task ListenAsync(WebSocket webSocket)
        {
            // Tạo một bộ đệm để nhận dữ liệu
            var buffer = new byte[1024 * 4];

            // Vòng lặp lắng nghe miễn là WebSocket còn mở
            while (webSocket.State == WebSocketState.Open)
            {
                // Nhận dữ liệu từ WebSocket
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Nếu tin nhắn là yêu cầu đóng kết nối
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Đóng kết nối WebSocket với trạng thái bình thường
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                    // Loại bỏ WebSocket khỏi danh sách kết nối
                    _connections.Remove(webSocket);
                }
            }
        }

        // Phương thức để gửi dữ liệu đến tất cả các WebSocket đang mở
        public async Task BroadcastAsync(string action, object data)
        {
            // Tạo tin nhắn dạng JSON từ action và data
            var message = JsonSerializer.Serialize(new { action, data });
            // Chuyển tin nhắn thành mảng byte
            var buffer = Encoding.UTF8.GetBytes(message);

            // Lặp qua tất cả kết nối WebSocket đang mở và gửi tin nhắn
            foreach (var connection in _connections.Where(c => c.State == WebSocketState.Open).ToList())
            {
                try
                {
                    // Gửi tin nhắn đến WebSocket
                    await connection.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch
                {
                    // Nếu có lỗi, loại bỏ kết nối khỏi danh sách
                    _connections.Remove(connection);
                }
            }
        }
    }

}
