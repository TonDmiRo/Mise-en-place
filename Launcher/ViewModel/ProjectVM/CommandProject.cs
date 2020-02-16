using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ViewModel {
    abstract class ProjectCommand {

        
        public abstract void Execute();
        public abstract void UnExecute();
    }
}
