using authapi.Data;
using authapi.DTOs;
using authapi.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace authapi.Services;

public class AuthService {
    private readonly MongoDbContext _mongoDb;
    private readonly PasswordHasher<User> _passwordHasher;
    public AuthService(MongoDbContext mongoDb) {
        _mongoDb = mongoDb;
        _passwordHasher = new PasswordHasher<User>();
    }
    public async Task<bool> RegisterAsync(RegisterRequest request) {
        var existingUser = await _mongoDb.Users
            .Find(user => user.Email == request.Email)
            .FirstOrDefaultAsync();

        if (existingUser != null) {
            return false;
        }

        var user = new User {
            Name = request.Name,
            Email = request.Email
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        await _mongoDb.Users.InsertOneAsync(user);

        return true;
    }

    public async Task<User?> LoginAsync(LoginRequest request) {
        var user = await _mongoDb.Users
            .Find(user => user.Email == request.Email)
            .FirstOrDefaultAsync();
        
        if (user == null) {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (result != PasswordVerificationResult.Success) return null;
        return user;
    }
}