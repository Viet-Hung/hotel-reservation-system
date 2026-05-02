namespace HotelReservation.Domain.Common;

/// <summary>
/// Exception cho business logic violations
/// Dùng để:
/// - Báo lỗi nghiệp vụ rõ ràng
/// - Phân biệt với technical errors
/// - Dễ handle ở Application layer
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Constructor với message
    /// </summary>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor với message + inner exception
    /// </summary>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Factory method để tạo exception với formatted message
    /// </summary>
    public static DomainException Create(string messageFormat, params object[] args)
    {
        return new DomainException(string.Format(messageFormat, args));
    }
}