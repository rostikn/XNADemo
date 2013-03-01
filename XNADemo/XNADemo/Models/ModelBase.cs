using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace XNADemo.Models
{
    internal class ModelBase
    {

        public ContentManager ContentManager { get; set; }

        public Model Model { get; private set; }

        public ModelBase(ContentManager contentManager, string meshFolderName, string modelFolderName, string modelName)
        {
            ContentManager = contentManager;
            MeshFolderName = meshFolderName;
            ModelFolderName = modelFolderName;
            ModelName = modelName;
            SpecularColor = new Vector3(defaultSpecularColorValue);
            SpecularPower = defaultSpecularPower;
            IsEnableDefaultLighting = defaultIsEnableDefaultLighting;
        }

        public string MeshFolderName { get; set; }
        public string ModelFolderName { get; set; }
        public string ModelName { get; set; }
        public string ModelFileName
        {
            get
            {
                return Path.Combine(MeshFolderName, ModelFolderName, ModelName);
            }
        }

        public void Load()
        {
            Model = ContentManager.Load<Model>(ModelFileName);
            ValidateModel();
        }

        const string cantFindModelExceptionMessageTemplate = "Can't find a model with this file name: {0}";
        protected virtual void ValidateModel()
        {
            if (Model == null)
            {
                throw new IOException(string.Format(cantFindModelExceptionMessageTemplate, ModelFileName));
            }
        }

        private float defaultSpecularColorValue = 0.25f;
        private const float defaultSpecularPower = 16;
        private const bool defaultIsEnableDefaultLighting = true;

        public Vector3 SpecularColor { get; set; }
        public float SpecularPower { get; set; }
        public bool IsEnableDefaultLighting { get; set; }

        public virtual void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            Matrix[] tranforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(tranforms);

            foreach (ModelMesh modelMesh in Model.Meshes)
            {
                Matrix world = tranforms[modelMesh.ParentBone.Index];

                InitializeMesh(modelMesh, world, view, projection);
                modelMesh.Draw();
            }
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh modelMesh in Model.Meshes)
            {
                InitializeMesh(modelMesh, world, view, projection);
                modelMesh.Draw();
            }
        }

        protected virtual void InitializeMesh(ModelMesh mesh, Matrix world, Matrix view, Matrix projection)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                InitializeEffect(effect, world, view, projection);
            }
        }

        protected virtual void InitializeEffect(BasicEffect effect, Matrix world, Matrix view, Matrix projection)
        {
            effect.View = view;
            effect.Projection = projection;
            effect.World = world;

            if (IsEnableDefaultLighting)
            {
                effect.EnableDefaultLighting();
            }

            effect.SpecularColor = SpecularColor;
            effect.SpecularPower = SpecularPower;
        }
    }
}
