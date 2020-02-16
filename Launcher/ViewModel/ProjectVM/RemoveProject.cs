using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ViewModel {
    class RemoveProject : ProjectCommand {
        protected ProjectReceiver receiver;
        protected Project project;
        public RemoveProject( ProjectReceiver receiver, Project project) {
            this.receiver = receiver;
            this.project = project;
        }
        public override void Execute() {
            receiver.Action();
        }

        public override void UnExecute() {
            throw new NotImplementedException();
        }
    }
}
