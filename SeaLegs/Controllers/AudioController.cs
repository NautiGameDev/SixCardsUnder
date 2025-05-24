using Microsoft.JSInterop;
using SeaLegs.Core;
using SeaLegs.Data;

namespace SeaLegs.Controllers
{
    public class AudioController : Controller
    {
        public static float MasterVolume { get; private set; } = 1f;
        public static float MusicVolume { get; private set; } = 1f;

        public static bool isMuted { get; private set; } = false;

        public static void SetMasterVolume(float volume)
        {
            MasterVolume = Math.Clamp(volume, 0f, 1f);
            CanvasController.JSModule.InvokeVoidAsync("updateMusicVolume", MasterVolume * MusicVolume);
            Console.WriteLine($"Changed master volume to {volume}");
        }

        public static void SetMusicVolume(float volume)
        {
            MusicVolume = Math.Clamp(volume, 0f, 1f);
            CanvasController.JSModule.InvokeVoidAsync("updateMusicVolume", MasterVolume * MusicVolume);
            Console.WriteLine($"Changed music volume to {volume}");
        }

        public static void MuteAllSound(bool value)
        {
            isMuted = value;
        }

        public static async Task PlaySound(string path)
        {
            if (isMuted) { return; }

            if (CanvasController.JSModule != null)
            {
                try
                {
                    await CanvasController.JSModule.InvokeVoidAsync("playSound", path, 1, false);
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"Error: Failed to play sound at {path}");
                    Console.WriteLine(ex.ToString());
                }  
                
            }
        }

        public static async Task PlaySound(string path, float volume)
        {
            if (isMuted) { return; }

            if (CanvasController.JSModule != null)
            {
                try
                {
                    await CanvasController.JSModule.InvokeVoidAsync("playSound", path, (MasterVolume * volume), false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to play sound at {path}");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static async Task PlaySound(string path, float volume, bool isLooping)
        {
            if (isMuted) { return; }

            if (CanvasController.JSModule != null)
            {
                try
                {
                    await CanvasController.JSModule.InvokeVoidAsync("playSound", path, (MasterVolume * volume), isLooping);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to play sound at {path}");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static async Task StopSound(string path)
        {
            if (CanvasController.JSModule != null)
            {
                try
                {
                    await CanvasController.JSModule.InvokeVoidAsync("stopSound", path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Sound at {path} can't be found in looping sounds dictionary.");
                }                
            }
        }
    }
}
