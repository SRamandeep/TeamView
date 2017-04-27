namespace WebRTCSignalR.Models
{
    public class User 
    {
        public string UserName;
        public string ConnectionId;
        public bool InCall;
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public bool HasAudioAvailbale { get; set; }
        public bool HasVideoAvaiable { get; set; }
        public bool IsVideoOn { get; set; }
        public bool IsAudioOn { get; set; }
    }
}