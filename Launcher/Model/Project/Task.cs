using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model {
    public class Task {
        [JsonProperty]
        public string TaskName { get; set; }
        [JsonProperty]
        public bool TaskStatus { get; set; }
        [JsonProperty]
        public DateTime DateOfCreation { get; private set; }
        public Task(string taskName) {
            TaskName = taskName;
            TaskStatus = false;
            DateOfCreation = DateTime.Now;
        }
    }
}
