namespace MonUX.Input
{
    /// <summary>
    /// Represents the changes in a button since the last poll time
    /// </summary>
    public enum ButtonDelta
    {
        /// <summary>
        /// This button has been released
        /// </summary>
        Released = 0x00,
        /// <summary>
        /// This button has been pressed
        /// </summary>
        Pressed = 0x01,
        /// <summary>
        /// This button was held down
        /// </summary>
        Down = 0x02,
        /// <summary>
        /// This button was left up
        /// </summary>
        Up = 0x03,

        /// <summary>
        /// The state of the button is unknown
        /// </summary>
        Unknown = 0x04,
        /// <summary>
        /// The state of the button is invalid
        /// </summary>
        Invalid = 0x05
    }

    /// <summary>
    /// Represents a button on a mouse that can be listened to
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// The left mouse button
        /// </summary>
        Left,
        /// <summary>
        /// The right mouse button
        /// </summary>
        Right,
        /// <summary>
        /// The middle mouse button
        /// </summary>
        Middle,
        /// <summary>
        /// The first eXtra button (typically forward)
        /// </summary>
        XButton1,
        /// <summary>
        /// The second eXtra button (typically back)
        /// </summary>
        XButton2
    }

    public enum InputType
    {
        Unknown = 0,
        Mouse = 1,
        Keyboard = 2,
        Controller = 3,
        Joystick = 4
    }
}
