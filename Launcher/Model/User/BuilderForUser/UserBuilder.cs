﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model.BuilderForUser {
    abstract class UserBuilder {
        public abstract void SetUserInformation();
        public abstract void SetProjects();
        public abstract void SetMaterials();
        public abstract User GetUser();
    }
}
