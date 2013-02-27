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
        SpriteBatch spriteBatch;
        
        // SkinnedModel objects
        AnimationPlayer animationPlayer;

        // Conent constants
        const string meshFolderName = "Mesh";
        const string diegoModelFileName = "diegoFixed.FBX";

        // Models
        Model diegoModel;

        public MainScene()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            diegoModel = this.Content.Load<Model>(Path.Combine(meshFolderName, diegoModelFileName));
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
            Viewport viewPort = GraphicsDevice.Viewport;
            float aspectRatio = (float)viewPort.Width / viewPort.Height;

            Matrix world, view, projection;

            Matrix[] transforms = new Matrix[diegoModel.Bones.Count];
            world = transforms[diegoModel.Meshes[0].ParentBone.Index];
            view = Matrix.CreateLookAt(new Vector3(0, 0, 500), 
                    Vector3.Zero, Vector3.Up) * Matrix.CreateTranslation(new Vector3(0, -100, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            diegoModel.CopyAbsoluteBoneTransformsTo(transforms);

            // TODO: Add your drawing code here
            foreach (ModelMesh modelMesh in diegoModel.Meshes)
            {
                foreach (BasicEffect effect in modelMesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                modelMesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
