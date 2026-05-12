using HotelReservation.Domain.Common;
using HotelReservation.Domain.Users.Events;
using HotelReservation.Domain.Users.ValueObjects;

namespace HotelReservation.Domain.Users.Entities;

/// <summary>
/// Aggregate Root đại diện cho user đã đăng ký trong hệ thống.
/// User có thể đăng nhập và tạo reservation.
/// </summary>
public sealed class User : Entity, IAggregateRoot
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public string FullName { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Constructor private cho EF Core materialization.
    private User()
    {
        Id = null!;
        Email = null!;
        Password = null!;
        FullName = null!;
    }

    private User(
        UserId id,
        Email email,
        Password password,
        string fullName,
        DateTime createdAt)
    {
        Id = id;
        Email = email;
        Password = password;
        FullName = fullName;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory method dùng cho use case đăng ký user.
    /// Đặt tên Register để thể hiện rõ business intent.
    /// </summary>
    public static User Register(
        Email email,
        Password hashedPassword,
        string fullName)
    {
        if (email is null)
            throw new DomainException("Email is required.");

        if (hashedPassword is null)
            throw new DomainException("Password is required.");

        ValidateFullName(fullName);

        var user = new User(
            UserId.CreateUnique(),
            email,
            hashedPassword,
            fullName.Trim(),
            DateTime.UtcNow);

        user.RaiseDomainEvent(new UserRegisteredEvent(
            user.Id,
            user.Email.Value));

        return user;
    }

    /// <summary>
    /// Cập nhật thông tin profile đơn giản.
    /// Hiện tại chỉ cho đổi FullName.
    /// </summary>
    public void UpdateProfile(string fullName)
    {
        ValidateFullName(fullName);

        FullName = fullName.Trim();
    }

    /// <summary>
    /// Đổi mật khẩu đã hash.
    /// Hashing/validate password strength sẽ xử lý ở Application/Infrastructure.
    /// </summary>
    public void ChangePassword(Password newHashedPassword)
    {
        if (newHashedPassword is null)
            throw new DomainException("Password is required.");

        Password = newHashedPassword;
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");

        var trimmedFullName = fullName.Trim();

        if (trimmedFullName.Length < 2 || trimmedFullName.Length > 100)
            throw new DomainException("Full name must be between 2 and 100 characters.");
    }
}