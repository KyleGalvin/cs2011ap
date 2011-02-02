using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.Graphics;


namespace AP
{
    public class Controls
    {
        Joystick joystick1;
        Surface screen;
        public Controls()
        {
            if( Joysticks.NumberOfJoysticks > 0 )
            {
                joystick1 = Joysticks.OpenJoystick(0);
                Console.WriteLine("axes: " + joystick1.NumberOfAxes);
                Console.WriteLine("number of balls:" + joystick1.NumberOfBalls);
                Console.WriteLine("number of buttons: " + joystick1.NumberOfButtons);
                Console.WriteLine("number of hats: " + joystick1.NumberOfHats);
                Events.JoystickBallMotion += new EventHandler<JoystickBallEventArgs>(Events_JoystickBallMotion);
                Events.JoystickAxisMotion += new EventHandler<JoystickAxisEventArgs>(Events_JoystickAxisMotion);
                Events.JoystickButtonDown += new EventHandler<JoystickButtonEventArgs>(Events_JoystickButtonDown);
                Events.JoystickButtonUp += new EventHandler<JoystickButtonEventArgs>(Events_JoystickButtonUp);
                Events.JoystickHatMotion += new EventHandler<JoystickHatEventArgs>(Events_JoystickHatMotion);
            }
            Events.KeyboardDown += new EventHandler<KeyboardEventArgs>(KeyDownEventHandler);
            Events.KeyboardUp += new EventHandler<KeyboardEventArgs>(KeyUpEventHandler);
            screen = Video.SetVideoMode(500, 500);
            Events.Run();
        }

        public void movePlayer()
        {

        }

        private static void Events_JoystickBallMotion(object sender, JoystickBallEventArgs args)
        {
            Console.WriteLine("Events_JoystickBallMotion");
        }

        private static void Events_JoystickAxisMotion(object sender, JoystickAxisEventArgs args)
        {
            Console.WriteLine("Events_JoystickAxisMotion");
        }
        private static void Events_JoystickButtonDown(object sender, JoystickButtonEventArgs args)
        {
            Console.WriteLine("button:" + args.Button);
            Console.WriteLine("Events_JoystickButtonDown");
        }
        private static void Events_JoystickButtonUp(object sender, JoystickButtonEventArgs args)
        {
            Console.WriteLine("Events_JoystickButtonUp");
        }
        private static void Events_JoystickHatMotion(object sender, JoystickHatEventArgs args)
        {
            Console.WriteLine(args.HatValue);
            Console.WriteLine(args.HatIndex);
            Console.WriteLine("Events_JoystickHatMotion");
        }

        private static void KeyDownEventHandler(object sender, KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case Key.UpArrow:
                    Console.WriteLine("Up arrow pressed down");
                    break;
                case Key.DownArrow:
                    Console.WriteLine("Down arrow pressed down");
                    break;
                case Key.LeftArrow:
                    Console.WriteLine("Left arrow pressed down");
                    break;
                case Key.RightArrow:
                    Console.WriteLine("Right arrow pressed down");
                    break;
                case Key.Escape:
                    Events.QuitApplication();
                    break;
            }
        }

        private static void KeyUpEventHandler(object sender, KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case Key.UpArrow:
                    Console.WriteLine("Up arrow up");
                    break;
                case Key.DownArrow:
                    Console.WriteLine("Down arrow up");
                    break;
                case Key.LeftArrow:
                    Console.WriteLine("Left arrow up");
                    break;
                case Key.RightArrow:
                    Console.WriteLine("Right arrow up");
                    break;
                case Key.Escape:
                    Events.QuitApplication();
                    break;
            }
        }

    }
}
