using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gửi thông báo tới client thông qua SignalR.
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="message">Nội dung thông báo</param>
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            Console.WriteLine(21);
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
            Console.WriteLine(22);

            return Ok(new { Message = "Notification sent successfully" });
        }
    }
}
