using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonUX.Render
{
    internal class MonuxServiceProvider : IServiceProvider
    {
        private GraphicsDeviceService _graphicsDevice;
        private Game _game;

        public Game Game
        {
            get { return _game; }
        }
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice.GraphicsDevice; }
        }

        public MonuxServiceProvider(Game game)
        {
            _graphicsDevice = new GraphicsDeviceService(game.GraphicsDevice);
            _game = game;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphicsDeviceService))
                return _graphicsDevice;
            else if (serviceType == typeof(Game))
                return _game;
            return null;
        }
    }

    internal class GraphicsDeviceService : IGraphicsDeviceService
    {
        GraphicsDevice _device;

        public GraphicsDeviceService(GraphicsDevice device)
        {
            _device = device;
            _device.DeviceReset += DeviceReset;
            _device.DeviceResetting += DeviceResetting;
            _device.Disposing += DeviceDisposing;
        }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return _device; }
        }
    }
}
