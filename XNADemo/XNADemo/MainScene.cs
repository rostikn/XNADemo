using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using SkinnedModel;

namespace XNADemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainScene : Microsoft.Xna.Framework.Game
    {
        // XNA objects
        GraphicsDeviceManager graphics;
        
        // SkinnedModel objects
        AnimationPlayer animationPlayer;
        SkinningData skinningData;
        Matrix[] bonesTransforms;

        // Conent constants
        const string contentFolderName = "Content";
        const string meshFolderName = "Mesh";
        const string mainModelName = "dude";

        // View variables
        float cameraArc = 0;
        float cameraRotation = 0;
        float cameraDistance = 100;

        // Models
        Model mainModel;

        public MainScene()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = contentFolderName;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here

            LoadMainModel();
            InitializeMainModelSkinningData();
            InitializeMainModelBonesTransforms();

            InitializeAnimationPlayer();
            
        }

        private void LoadMainModel()
        {
            string mainModelPath = Path.Combine(meshFolderName, mainModelName);
            mainModel = this.Content.Load<Model>(mainModelPath);

            if (mainModel == null)
            {
                throw new IOException(
                    string.Format("Can't find a model by a specified path", mainModelPath)
                );
            }
        }

        private void InitializeMainModelSkinningData()
        {
            skinningData = mainModel.Tag as SkinningData;
            if (skinningData == null)
            {
                throw new InvalidOperationException("This model does not contain SkinningData tag.");
            }
        }

        private void InitializeMainModelBonesTransforms()
        {
            bonesTransforms = new Matrix[skinningData.BindPose.Count];
        }

        private void InitializeAnimationPlayer()
        {
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];
            animationPlayer.StartClip(clip);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            animationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

            animationPlayer.GetBoneTransforms().CopyTo(bonesTransforms, 0);
            animationPlayer.UpdateWorldTransforms(Matrix.Identity, bonesTransforms);
            animationPlayer.UpdateSkinTransforms();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix[] bones = animationPlayer.GetSkinTransforms();

            Matrix view = Matrix.CreateTranslation(0, -40, 0) *
                   Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                   Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) * Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                   new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 10000);
            

            // TODO: Add your drawing code here
            foreach (ModelMesh modelMesh in mainModel.Meshes)
            {
                foreach (SkinnedEffect effect in modelMesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.SpecularColor = new Vector3(0.25f);

                    effect.SpecularPower = 16;
                }
                modelMesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
