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

        // DateTime objects
        DateTime previousUpdateTime;

        // SkinnedModel objects
        AnimationPlayer animationPlayer;
        SkinningData skinningData;
        Matrix[] bonesTransforms;

        // Input objects
        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState previousKeyboardState = new KeyboardState();

        // Conent constants
        const string contentFolderName = "Content";
        const string meshFolderName = "Mesh";
        const string audioFolderName = "Audio";
        

        // Camera constants
        const float defaultCameraArc = 0;
        const float defaultCameraRotation = 0;
        const float defaultCameraDistance = 100;

        // Camera variables
        float cameraArc = defaultCameraArc;
        float cameraRotation = defaultCameraRotation;
        float cameraDistance = defaultCameraDistance;

        // MediaPlayer constants
        const float defaultMediaPlayerVolume = 0.1f;

        // MediaPlayer variables
        float mediaPlayerVolume = defaultMediaPlayerVolume;

        // Songs
        const string mainThemeName = "MainTheme";
        Song mainTheme;

        // Models
        const string mainModelName = "dude";
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
            LoadMainTheme();

            LoadMainModel();
            InitializeMainModelSkinningData();
            InitializeMainModelBonesTransforms();

            InitializeAnimationPlayer();
            InitializeMediaPlayer();
        }

        private void LoadMainTheme()
        {
            string mainThemePath = Path.Combine(audioFolderName, mainThemeName);
            mainTheme = Content.Load<Song>(mainThemePath);
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

        private void InitializeMediaPlayer()
        {
            const bool isRepeating = true;

            MediaPlayer.IsRepeating = isRepeating;
            MediaPlayer.Play(mainTheme);
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
            HandleInput(gameTime);
            UpdateTransforms(gameTime);
            UpdateMediaPlayer();
            
            base.Update(gameTime);
        }

        #region Input Handlers
        private void HandleInput(GameTime gameTime)
        {
            ProcessKeyboardStates();

            HandleExit();

            HandleCameraArc(gameTime);
            HandleCameraRotation(gameTime);
            HandleCameraZoom(gameTime);

            HandleMediaPlayer(gameTime);

            HandleReset();
        }

        #region Process States
        private void ProcessKeyboardStates()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
        #endregion

        #region Handle Exit
        private void HandleExit()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }
        #endregion

        #region Handle Camera Up/Down
        private void HandleCameraArc(GameTime gameTime)
        {
            HandleCameraArcUp(gameTime);
            HandleCameraArcDown(gameTime);
            HandleCameraArcLimits();
        }

        private void HandleCameraArcUp(GameTime gameTime)
        {
            const float deltaCameraArc = 0.1f;

            float time = (float)gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentKeyboardState.IsKeyDown(Keys.W))
            {
                cameraArc += time * deltaCameraArc;
            }
        }

        private void HandleCameraArcDown(GameTime gameTime)
        {
            const float deltaCameraArc = 0.1f;

            float time = (float)gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentKeyboardState.IsKeyDown(Keys.W))
            {
                cameraArc -= time * deltaCameraArc;
            }
        }

        private void HandleCameraArcLimits()
        {
            const float cameraArcMaxLimit = 90.0f;
            const float cameraArcMinLimit = -90.0f;

            if (cameraArc > cameraArcMaxLimit)
                cameraArc = cameraArcMaxLimit;
            else if (cameraArc < cameraArcMinLimit)
                cameraArc = cameraArcMinLimit;
        }
        #endregion

        #region Handle Camera Rotation
        private void HandleCameraRotation(GameTime gameTime)
        {
            HandleCameraRotationLeft(gameTime);
            HandleCameraRotationRight(gameTime);
        }

        private void HandleCameraRotationLeft(GameTime gameTime)
        {
            const float deltaCameraRotation = 0.1f;

            float time = gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
               currentKeyboardState.IsKeyDown(Keys.A))
            {
                cameraRotation -= time * deltaCameraRotation;
            }
        }

        private void HandleCameraRotationRight(GameTime gameTime)
        {
            const float deltaCameraRotation = 0.1f;

            float time = gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentKeyboardState.IsKeyDown(Keys.D))
            {
                cameraRotation += time * deltaCameraRotation;
            }
        }
        #endregion

        #region Handle Camera Distance
        private void HandleCameraZoom(GameTime gameTime)
        {
            HandleCameraZoomIn(gameTime);
            HandleCameraZoomOut(gameTime);
            HandleCameraZoomLimits();
        }

        private void HandleCameraZoomOut(GameTime gameTime)
        {
            const float deltaCameraDistance = 0.25f;

            float time = gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                cameraDistance += time * deltaCameraDistance;
            }
        }

        private void HandleCameraZoomIn(GameTime gameTime)
        {
            const float deltaCameraDistance = 0.25f;

            float time = gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.X))
            {
                cameraDistance -= time * deltaCameraDistance;
            }
        }

        private void HandleCameraZoomLimits()
        {
            const float cameraDistanceMaxLimit = 500.0f;
            const float cameraDistanceMinLimit = 10.0f;

            if (cameraDistance > cameraDistanceMaxLimit)
            {
                cameraDistance = cameraDistanceMaxLimit;
            }
            else if (cameraDistance < cameraDistanceMinLimit)
            {
                cameraDistance = cameraDistanceMinLimit;
            }
        }
        #endregion

        #region Handle MediaPlayer
        private void HandleMediaPlayer(GameTime gameTime)
        {
            const int deltaTime = 100;
            if (DateTime.Now.Subtract(previousUpdateTime).TotalMilliseconds > deltaTime)
            {
                HandleMediaPlayerState();
                HandleMediaPlayerVolume();

                previousUpdateTime = DateTime.Now;
            }
        }
        #endregion

        #region Handle MediaPlayer State
        private void HandleMediaPlayerState()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Multiply) &&
                !previousKeyboardState.GetPressedKeys().Contains(Keys.Multiply))
            {
                ToggleMediaPlayer();
            }
        }
        private void ToggleMediaPlayer()
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
        }
        #endregion

        #region Hanlde Media Player Volume
        private void HandleMediaPlayerVolume()
        {
            HandleMediaPlayerVolumeIncrease();
            HandleMediaPlayerVolumeDecrease();
            HandleMediaPlayerVolumeLimits();
        }

        private void HandleMediaPlayerVolumeIncrease()
        {
            const float deltaMediaPlayerVolume = 0.05f;

            if (currentKeyboardState.IsKeyDown(Keys.Add))
            {
                mediaPlayerVolume += deltaMediaPlayerVolume;
            }
        }

        private void HandleMediaPlayerVolumeDecrease()
        {
            const float deltaMediaPlayerVolume = 0.05f;

            if (currentKeyboardState.IsKeyDown(Keys.Subtract))
            {
                mediaPlayerVolume -= deltaMediaPlayerVolume;
            }
        }

        private void HandleMediaPlayerVolumeLimits()
        {
            float mediaPlayerVolumeMaxLimit = 1.0f;
            float mediaPlayerVolumeMinLimit = 0.0f;

            if (mediaPlayerVolume > mediaPlayerVolumeMaxLimit)
                mediaPlayerVolume = mediaPlayerVolumeMaxLimit;
            else if (mediaPlayerVolume < mediaPlayerVolumeMinLimit)
                mediaPlayerVolume = mediaPlayerVolumeMinLimit;

        }

        #endregion

        #region Handle Reset
        private void HandleReset()
        {
            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                SetDefaultCameraValues();
                SetDefaultMediaPlayerValues();
            }
        }

        private void SetDefaultCameraValues()
        {
            cameraArc = defaultCameraArc;
            cameraRotation = defaultCameraRotation;
            cameraDistance = defaultCameraDistance;
        }

        private void SetDefaultMediaPlayerValues()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }
        #endregion
        
        #endregion

        private void UpdateTransforms(GameTime gameTime)
        {
            animationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

            animationPlayer.GetBoneTransforms().CopyTo(bonesTransforms, 0);
            animationPlayer.UpdateWorldTransforms(Matrix.Identity, bonesTransforms);
            animationPlayer.UpdateSkinTransforms();
        }

        private void UpdateMediaPlayer()
        {
            MediaPlayer.Volume = mediaPlayerVolume;
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
                   Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) * 
                   Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
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

        public float deltaMediaPlayerVolume { get; set; }
    }
}
