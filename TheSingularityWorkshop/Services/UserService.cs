// TheSingularityWorkshop/Services/UserService.cs

using System;

namespace TheSingularityWorkshop.Services
{
    public class UserService
    {
        public string CurrentUsername { get; private set; } = "Anonymous";

        // Declared as nullable Action? to prevent non-nullable field warning/error
        public event Action? OnStateChanged;

        public UserService()
        {
            // Initialize as Anonymous by default
            Login("Anonymous");
        }

        public void Login(string username)
        {
            string newUsername = string.IsNullOrWhiteSpace(username) ? "Anonymous" : username;

            if (CurrentUsername != newUsername)
            {
                CurrentUsername = newUsername;

                // Notify subscribed UI components to re-render.
                OnStateChanged?.Invoke();
            }
        }

        public void Logout()
        {
            Login("Anonymous");
        }

        public bool IsAnonymous()
        {
            return CurrentUsername.Equals("Anonymous", StringComparison.OrdinalIgnoreCase);
        }
    }
}