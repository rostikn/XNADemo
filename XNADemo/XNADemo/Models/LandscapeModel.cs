using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace XNADemo.Models
{
    internal class LandscapeModel : ModelBase
    {
        public LandscapeModel(ContentManager contentManager, string meshFolderName, string modelFolderName, string modelName) :
            base(contentManager, meshFolderName, modelFolderName, modelName) { }
    }
}
