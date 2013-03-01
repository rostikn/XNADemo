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
using XNADemo.Models;

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

        // Input objects
        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState previousKeyboardState = new KeyboardState();

        // Conent constants
        const string contentFolderName = "Content";
        const string meshFolderName = "Mesh";
        const string audioFolderName = "Audio";
        

        // Camera constants
        const float defaultCameraArc = 0;
        const float defaultCameraRotation = -45;
        const float defaultCameraDistance = 150;

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
        const string mainModelFolderName = "Dude";
        const string mainModelName = "dude";
        Model mainModel;
        SkinningData mainModelSkinningData;
        Matrix[] mainModelBonesTransforms;

        const string landscapeModelFolderName = "Landscape";
        const string landscapeModelName = "Level0";
        LandscapeModel landscapeModel;
        // New Models
        const string skyBoxModelFolderName = "SkyBox";
        const string skyBoxModelName = "skybox";
        SkyBoxModel skyBoxModel;

        // Models positions
        const int defaultMainModelAngle = 0;
        const int defaultMainModelPositionX = 0;
        const int defaultMainModelPositionY = 0;
        Point mainModelPosition = new Point(defaultMainModelPositionX, defaultMainModelPositionY);
        int mainModelAngle = defaultMainModelAngle;

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
            skyBoxModel = new SkyBoxModel(this.Content, meshFolderName, skyBoxModelFolderName, skyBoxModelName);
            landscapeModel = new LandscapeModel(this.Content, meshFolderName, landscapeModelFolderName, landscapeModelName);

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

            InitializeModels();

            InitializeAnimationPlayer();
            InitializeMediaPlayer();
        }

        private void InitializeModels()
        {
            LoadMainModel();
            InitializeMainModelSkinningData();
            InitializeMainModelBonesTransforms();

            LoadLandscapeModel();
            LoadSkyBoxModel();
            //InitializeLandscapeModelSkinningData();
        }

        private void LoadMainTheme()
        {
            string mainThemePath = Path.Combine(audioFolderName, mainThemeName);
            mainTheme = Content.Load<Song>(mainThemePath);
        }

        const string cantFindModelExceptionMessageTemplate = "Can't find a model by a specified path: {0}";
        private void LoadMainModel()
        {
            string mainModelPath = Path.Combine(meshFolderName, mainModelFolderName, mainModelName);
            mainModel = this.Content.Load<Model>(mainModelPath);

            if (mainModel == null)
            {
                throw new IOException(
                    string.Format(cantFindModelExceptionMessageTemplate, mainModelPath)
                    );
            }
        }

        const string invalidSkinningDataExceptionMessageTemplate = "This model does not contain SkinningData tag";
        private void InitializeMainModelSkinningData()
        {
            mainModelSkinningData = mainModel.Tag as SkinningData;
            if (mainModelSkinningData == null)
            {
                throw new InvalidOperationException(invalidSkinningDataExceptionMessageTemplate);
            }
        }

        private void InitializeMainModelBonesTransforms()
        {
            mainModelBonesTransforms = new Matrix[mainModelSkinningData.BindPose.Count];
        }

        private void LoadLandscapeModel()
        {
            landscapeModel.Load();
        }

        private void LoadSkyBoxModel()
        {
            skyBoxModel.Load();
        }


        private void InitializeAnimationPlayer()
        {
            animationPlayer = new AnimationPlayer(mainModelSkinningData);
            AnimationClip clip = mainModelSkinningData.AnimationClips["Take 001"];
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

        bool isFirstUpdateTRansformsCompleted = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!isFirstUpdateTRansformsCompleted)
            {
                UpdateTransforms(gameTime);
                isFirstUpdateTRansformsCompleted = true;
            }
            HandleInput(gameTime);
            UpdateSkyBox();
            UpdateMediaPlayer();
            
            
            base.Update(gameTime);
        }

        #region Input Handlers

        private void HandleInput(GameTime gameTime)
        {
            ProcessKeyboardStates();

            HandleExit();

            HandleMainModelPosition(gameTime);

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

        #region Handel Main Model Position

        private void HandleMainModelPosition(GameTime gameTime)
        {
            HandleMainModelGoForward(gameTime);
            //HandleMainModelGoBack(gameTime);
            HandleMainModelRotateLeft(gameTime);
            HandleMainModelRotateRight(gameTime);
            HandleMainModelPositionLimits();
        }

        private void HandleMainModelGoForward(GameTime gameTime)
        {
            if(currentKeyboardState.IsKeyDown(Keys.W))
            {
                float mainModelPositionAngleAsFloat = mainModelAngle;
                mainModelPosition.Y += (int)(Math.Sin(MathHelper.ToRadians(mainModelPositionAngleAsFloat + 90)) * 10) / 5;
                mainModelPosition.X += (int)(Math.Cos(MathHelper.ToRadians(mainModelPositionAngleAsFloat + 90)) * 10) / 5;
                UpdateTransforms(gameTime);
            }
        }

        private void HandleMainModelGoBack(GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                mainModelPosition.Y--;
                UpdateTransforms(gameTime);
            }
        }

        private void HandleMainModelRotateLeft(GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                mainModelAngle++;
                UpdateTransforms(gameTime);
            }
        }

        private void HandleMainModelRotateRight(GameTime gameTime)
        {
            if(currentKeyboardState.IsKeyDown(Keys.D))
            {
                mainModelAngle--;
                UpdateTransforms(gameTime);
            }
        }

        private void HandleMainModelPositionLimits()
        {
            HandleMainModelPositionXLimits();
            HandleMainModelPositionYLimits();
        }

        private void HandleMainModelPositionXLimits()
        {
            const int maxMainModelXPosition = 500;
            const int minMainModelXPosition = -500;

            if (mainModelPosition.X > maxMainModelXPosition)
            {
                mainModelPosition.X = maxMainModelXPosition;
            }
            else if (mainModelPosition.X < minMainModelXPosition)
            {
                mainModelPosition.X = minMainModelXPosition;
            }
        }

        private void HandleMainModelPositionYLimits()
        {
            const int maxMainModelYPosition = 500;
            const int minMainModelYPosition = -500;
            if (mainModelPosition.Y > maxMainModelYPosition)
            {
                mainModelPosition.Y = maxMainModelYPosition;
            }
            else if (mainModelPosition.Y < minMainModelYPosition)
            {
                mainModelPosition.Y = minMainModelYPosition;
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

            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                cameraArc += time * deltaCameraArc;
            }
        }

        private void HandleCameraArcDown(GameTime gameTime)
        {
            const float deltaCameraArc = 0.1f;

            float time = (float)gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                cameraArc -= time * deltaCameraArc;
            }
        }

        private void HandleCameraArcLimits()
        {
            const float cameraArcMaxLimit = 0.0f;
            const float cameraArcMinLimit = -10.0f;

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

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                cameraRotation -= time * deltaCameraRotation;
            }
        }

        private void HandleCameraRotationRight(GameTime gameTime)
        {
            const float deltaCameraRotation = 0.1f;

            float time = gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.Right))
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
            const float cameraDistanceMaxLimit = 750.0f;
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

        #region Updates

        private void UpdateTransforms(GameTime gameTime)
        {
            animationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

            animationPlayer.GetBoneTransforms().CopyTo(mainModelBonesTransforms, 0);
            Matrix rotationX = Matrix.CreateRotationY(mainModelAngle * MathHelper.Pi / 180);
            animationPlayer.UpdateWorldTransforms(rotationX, mainModelBonesTransforms);
            animationPlayer.UpdateSkinTransforms();
        }

        private void UpdateSkyBox()
        {
            //skyBoxModel.Model.Meshes
        }

        private void UpdateMediaPlayer()
        {
            MediaPlayer.Volume = mediaPlayerVolume;
        }

        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Matrix mainModelTranslation = MainModelTranslation();
            Matrix view = CreateView();
            Matrix projection = CreateProjection();

            // TODO: Add your drawing code here
            double skyWorldX = cameraDistance * Math.Cos(MathHelper.ToRadians(cameraRotation));
            double skyWorldY = cameraDistance * Math.Sin(MathHelper.ToRadians(cameraRotation));
            Matrix skyBoxWorld = Matrix.CreateTranslation((float)skyWorldY, (float)skyWorldX, 0);

            skyBoxModel.Draw(GraphicsDevice, skyBoxWorld, view, projection);

            landscapeModel.Draw(GraphicsDevice, view, projection);

            DrawMainModel(mainModelTranslation, view, projection);

            base.Draw(gameTime);
        }

        private Matrix MainModelTranslation()
        {
            const float mainModelOffsetZ = 3f;
            return Matrix.CreateTranslation(mainModelPosition.X, mainModelOffsetZ, -mainModelPosition.Y);
        }

        private Matrix CreateView()
        {
            return Matrix.CreateTranslation(0, -40, 0) *
                   Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                   Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                   Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                    new Vector3(0, 0, 0), Vector3.Up);
        }

        private Matrix CreateProjection()
        {
            return Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 2, 10000);
        }


        private void DrawMainModel(Matrix world, Matrix view, Matrix projection)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();

            foreach (ModelMesh modelMesh in mainModel.Meshes)
            {
                foreach (SkinnedEffect effect in modelMesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.SpecularColor = new Vector3(0.25f);

                    effect.SpecularPower = 16;
                }
                modelMesh.Draw();
            }
        }


        public float deltaMediaPlayerVolume { get; set; }
    }
}
