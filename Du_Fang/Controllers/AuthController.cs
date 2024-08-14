using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Du_Fang.Services;


namespace Du_Fang.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthControllerOTP : ControllerBase
{
    //dependency injection
    private readonly EmailSender _emailSender;
    private readonly AppDBContext _context;

    //constructor to initialise AuthController with needed dependencies
    public AuthControllerOTP(AppDBContext context, EmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }



    //TODO: test the sending of our email
    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        string toEmail = "221345@virtualwindow.co.za";
        string subject = "Test Email";
        string body = "This is a test email to verify email sending functionality.";

        await _emailSender.SendEmailAsync(toEmail, body, subject);
        return Ok("Test email sent.");
    }


    //TODO: API to generate and save the OTP (send the email) - login
    [HttpPost("generate—otp")]
    public async Task<IActionResult> GenerateOtp(string email)
    {
        //Check to make sure that this email exists as a user in our table
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null) return NotFound("User not found");

        user.GenerateOTP(); //Call the functionality and save it for the user that we found in the object
        await _context.SaveChangesAsync(); //sync the new data to our tables in the db

        var otpMsg = $"Your OTP is {user.Otp}. It will expire in 5 min.";

        await _emailSender.SendEmailAsync(user.Email, otpMsg, "One Time Pin for Du_Fang"); //sending otp via email

        //encrypt otp before saving
        //TODO SELF: encrypt the OTP - Argon2 (own research)

        return Ok("OTP Sent.");
    }


    //TODO: API validate if the user has entered the correct OTP
    [HttpPost("validate—otp")]
    public async Task<IActionResult> ValidateOtp(OtpEmail otpEmail)
    {
        //Check to make sure that this email exists as a user in our table
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == otpEmail.Email);
        if (user == null) return NotFound("User not found");

        if (user.ValidateOTP(otpEmail.Otp))
        {
            //valid otp
            // TODO self (optional): JWT - pass the token here
            return Ok("OTP is valid. Let me into the site.");
        }
        else
        {
            return BadRequest("Invalid OTP.");
        }
    }
    public class OtpEmail
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
