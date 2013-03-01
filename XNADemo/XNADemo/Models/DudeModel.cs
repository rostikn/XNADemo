using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Cybertone.XNA40Demo.Models
{
    internal class DudeModel : ModelBase
    {
        public DudeModel(ContentManager contenetManager, string meshFolderName, string modelFolderName, string modelName) : 
            base(contenetManager, meshFolderName, modelFolderName, modelName) { }
    }
}
