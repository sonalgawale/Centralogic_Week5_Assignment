using Newtonsoft.Json;

namespace Week5_Sonal_Gawale.DTO
{
    public class TaskModel
    {
        [JsonProperty(PropertyName = "uId", NullValueHandling = NullValueHandling.Ignore)]
        public string UId { get; set; }

        [JsonProperty(PropertyName = "taskId", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskId { get; set; }

        [JsonProperty(PropertyName = "taskTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskTitle { get; set; }

        [JsonProperty(PropertyName = "taskDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskDescription { get; set; }
    }
}
