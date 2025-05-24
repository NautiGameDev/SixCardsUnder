using System.Numerics;
using System.Runtime.CompilerServices;
using SeaLegs.Core;

namespace SeaLegs.Controllers
{
    public class InputController : Controller
    {
        //Contains data of keys being pressed as opposed to single key down and key up event
        

        public static Dictionary<string, bool> KeysPressed { get; private set; } = new Dictionary<string, bool>();
        public static Queue<string> KeyDownEvent { get; private set; } = new Queue<string>();
        public static Queue<string> KeyUpEvent { get; private set; } = new Queue<string>();

        public static Vector2 MousePosition { get; private set; }
        public static bool MouseClickDown { get; private set; } = false;
        public static Vector2 MouseDownCoords { get; private set; }
        public static bool MouseClickUp { get; private set; } = false;
        public static Vector2 MouseUpCoords { get; private set; }

        

        #region Keyboard Input
        //Handle keys held down
        public static void SetKeyPressed(string key, bool value)
        {
            KeysPressed[key] = value;
        }

        public static bool IsKeyPressed(string key, bool value)
        {
            return KeysPressed[key];
        }

        //Handle single key presses
        public static void AddKeyDown(string key)
        {
            

            KeyDownEvent.Clear(); //Hot fix clears queue each time a key is pressed.
            if (!KeyDownEvent.Contains(key))
            {
                KeyDownEvent.Enqueue(key);
            }            
        }

        public static bool OnKeyDown(string key)
        {
            /*Used within game logic to detect single key presses. Dequeue first item in queue regardless to cycle the queue*/
            if (KeyDownEvent.Count > 0 && KeyDownEvent.Peek() == key)
            {
                KeyDownEvent.Dequeue();
                return true;
            }
                
            return false;
        }

        //Handle key up events
        public static void AddKeyUp(string key)
        {
            KeyUpEvent.Clear(); //Hot fix clears queue each time a key is pressed.
            if (!KeyUpEvent.Contains(key))
            {
                   KeyUpEvent.Enqueue(key);
            }
        }

        public static bool OnKeyUp(string key)
        {
            /*Used within game logic to detect single key presses. Dequeue first item in queue regardless to cycle the queue*/

            if (KeyUpEvent.Count > 0 && KeyUpEvent.Peek() == key)
            {
                KeyUpEvent.Dequeue();
                return true;
            }
            return false;
        }
        #endregion

        public static void SetMousePosition(Vector2 position)
        {
            MousePosition = position;
        }

        public static Vector2 GetMousePosition()
        {
            return MousePosition;
        }

        public static void SetMouseDown(bool value)
        {
            if(value)
            {
                MouseDownCoords = new Vector2(MousePosition.X, MousePosition.Y);
            }
            
            MouseClickDown = value;
        }

        public static void SetMouseUp(bool value)
        {
            if(value)
            {
                MouseUpCoords = new Vector2(MousePosition.X, MousePosition.Y);
            }
            
            MouseClickUp = value;
        }

        public static bool OnMouseDown()
        {
            bool ValueToReturn = MouseClickDown;
            MouseClickDown = false;
            return ValueToReturn;
        }

        public static bool OnMouseUp()
        {
            bool ValueToReturn = MouseClickUp;
            MouseClickUp = false;
            return ValueToReturn;
        }
    }
}
