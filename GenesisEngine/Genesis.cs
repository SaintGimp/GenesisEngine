using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StructureMap;

namespace GenesisEngine
{
    public class Genesis : Game,
                           IListener<ExitApplication>,
                           IListener<GarbageCollect>
    {
        private MainPresenter _mainPresenter;
        private IInputState _inputState;
        IInputMapper _inputMapper;

        public Genesis()
        {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            GraphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            SetViewportDependentParameters();
        }

        private void SetViewportDependentParameters()
        {
            var width = GraphicsDevice.Viewport.Width;
            var height = GraphicsDevice.Viewport.Height;
            
            _mainPresenter.SetViewportSize(width, height);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            _mainPresenter = ObjectFactory.GetInstance<MainPresenter>();
            _mainPresenter.Show();

            _inputState = ObjectFactory.GetInstance<IInputState>();
            _inputMapper = ObjectFactory.GetInstance<IInputMapper>();
            
            SetInputBindings();

            base.Initialize();
        }

        // Temporarily here until InputMapper can load its own settings
        void SetInputBindings()
        {
            _inputMapper.AddKeyDownMessage<MoveForward>(Keys.W);
            _inputMapper.AddKeyDownMessage<MoveBackward>(Keys.S);
            _inputMapper.AddKeyDownMessage<MoveLeft>(Keys.A);
            _inputMapper.AddKeyDownMessage<MoveRight>(Keys.D);
            _inputMapper.AddKeyDownMessage<MoveUp>(Keys.E);
            _inputMapper.AddKeyDownMessage<MoveDown>(Keys.C);
            _inputMapper.AddKeyDownMessage<GoToGround>(Keys.Z);

            _inputMapper.AddKeyPressMessage<ToggleDrawWireframeSetting>(Keys.F);
            _inputMapper.AddKeyPressMessage<ToggleUpdateSetting>(Keys.U);
            _inputMapper.AddKeyPressMessage<ToggleSingleStepSetting>(Keys.P);
            _inputMapper.AddKeyPressMessage<IncreaseCameraSpeed>(Keys.OemPlus);
            _inputMapper.AddKeyPressMessage<DecreaseCameraSpeed>(Keys.OemMinus);

            _inputMapper.AddKeyPressMessage<GarbageCollect>(Keys.G);

            _inputMapper.AddKeyPressMessage<ExitApplication>(Keys.Escape);

            // TODO: we don't specify which mouse button must be down (hardcoded to right button ATM),
            // this can be extended when we need to.
            _inputMapper.AddMouseMoveMessage<MouseLook>();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SetViewportDependentParameters();
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
            _inputState.Update(gameTime.ElapsedGameTime, Keyboard.GetState(), Mouse.GetState());
            _inputMapper.HandleInput(_inputState);

            Debug.WriteLine(Stopwatch.StartNew().Measure(() => _mainPresenter.Update(gameTime.ElapsedGameTime)));
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            _mainPresenter.Draw();

            base.Draw(gameTime);
        }

        public void Handle(ExitApplication message)
        {
            Exit();
        }

        public void Handle(GarbageCollect message)
        {
            GC.Collect();
        }
    }
}
