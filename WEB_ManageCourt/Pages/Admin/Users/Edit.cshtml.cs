﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic.Interface;
using Model;

namespace WEB_ManageCourt.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("User.Password");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var existingUser = await _userService.GetUserByIdAsync(User.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }
            User.Password = existingUser.Password;
            try
            {
                await _userService.UpdateUserAsync(User);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the user: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Index");
            }
        }
    
}
