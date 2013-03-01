using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XNADemo.Models
{
    internal class SkyBoxModel : ModelBase
    {
        public SkyBoxModel(ContentManager contentManager, string meshFolderName, string modelFolderName, string modelName) : 
            base(contentManager, meshFolderName, modelFolderName, modelName) { }

        public override void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {

            Vector3 xwingPosition = new Vector3(8, -2600, -3);
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            graphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            graphicsDevice.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(xwingPosition);
                    //currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.World = worldMatrix;
                    currentEffect.View = view;
                    currentEffect.Projection = projection;
                    //currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            graphicsDevice.DepthStencilState = dss;
        }
    }
}
