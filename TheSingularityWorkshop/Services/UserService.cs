using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TheSingularityWorkshop.Services
{
    public class UserService
    {
        private readonly AuthenticationStateProvider _authProvider;

        // Backed by Azure, but we keep the property for compatibility
        public string CurrentUsername { get; private set; } = "Anonymous";
        public bool IsLoggedIn => !IsAnonymous();

        // FIX 1: Make event nullable (?) to silence the constructor warning
        public event Action? OnStateChanged;

        public UserService(AuthenticationStateProvider authProvider)
        {
            _authProvider = authProvider;
            _authProvider.AuthenticationStateChanged += AuthStateChanged;

            // Fire and forget the update
            _ = UpdateUserAsync();
        }

        private async void AuthStateChanged(Task<AuthenticationState> task) => await UpdateUserAsync();

        private async Task UpdateUserAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            var user = state.User;

            CurrentUsername = (user.Identity != null && user.Identity.IsAuthenticated)
                ? user.Identity.Name ?? "Unknown"
                : "Anonymous";

            OnStateChanged?.Invoke();
        }

        public bool IsAnonymous()
        {
            return CurrentUsername.Equals("Anonymous", StringComparison.OrdinalIgnoreCase);
        }

        // --- COMPATIBILITY STUBS (Fixes the 'Definition not found' errors) ---
        // These methods are called by your old components. 
        // We leave them empty because Azure handles the actual login logic now.

        public void Login(string username)
        {
            // Optional: Log that this happened
            Console.WriteLine($"[UserService] Manual Login for '{username}' ignored. Auth is handled by Azure.");
        }

        public void Logout()
        {
            Console.WriteLine("[UserService] Manual Logout ignored. Auth is handled by Azure.");
        }
    }
}