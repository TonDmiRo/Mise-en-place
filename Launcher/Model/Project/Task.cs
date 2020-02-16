using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model {
    public class Task {
        public string TaskName { get; set; }
        public bool TaskStatus { get; set; }
        public DateTime DateOfCreation { get; private set; }
        public Task(string taskName) {
            TaskName = taskName;
            TaskStatus = false;
            DateOfCreation = DateTime.Now;
        }
    }
}
