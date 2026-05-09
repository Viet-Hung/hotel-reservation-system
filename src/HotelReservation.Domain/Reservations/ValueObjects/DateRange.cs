using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Reservations.ValueObjects;

/// <summary>
/// Value Object đại diện cho khoảng thời gian lưu trú
/// Immutable - không thể thay đổi sau khi tạo
/// </summary>
public sealed class DateRange : ValueObject
{
    /// <summary>
    /// Ngày check-in
    /// </summary>
    public DateTime CheckIn { get; private set; }

    /// <summary>
    /// Ngày check-out
    /// </summary>
    public DateTime CheckOut { get; private set; }

    /// <summary>
    /// Số đêm lưu trú (tính toán từ CheckIn và CheckOut)
    /// VD: CheckIn = 1/1/2024, CheckOut = 3/1/2024 → 2 đêm
    /// </summary>
    public int Nights => (CheckOut.Date - CheckIn.Date).Days;

    /// <summary>
    /// Private constructor để force dùng Create factory method
    /// </summary>
    private DateRange(DateTime checkIn, DateTime checkOut)
    {
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    /// <summary>
    /// Factory method để tạo DateRange với validation
    /// Tại sao dùng factory method thay vì constructor public?
    /// - Có thể return null hoặc throw exception khi invalid
    /// - Tên method rõ ràng hơn "new DateRange()"
    /// - Có thể thêm logic phức tạp (VD: cache, logging)
    /// </summary>
    public static DateRange Create(DateTime checkIn, DateTime checkOut)
    {
        // Business Rule 1: CheckIn phải là ngày trong tương lai hoặc hôm nay
        // Test case: Chuyển rule sang Reservation.Create: để unit test trong quá khứ vẫn chạy được
        // if (checkIn.Date < DateTime.Today)
        // {
        //     throw new DomainException("Check-in date cannot be in the past.");
        // }

        // Business Rule 2: CheckOut phải sau CheckIn
        if (checkOut.Date <= checkIn.Date)
        {
            throw new DomainException("Check-out date must be after check-in date.");
        }

        // Business Rule 3: Tối thiểu 1 đêm, tối đa 30 đêm
        var nights = (checkOut.Date - checkIn.Date).Days;
        if (nights > 30)
        {
            throw new DomainException("Maximum stay is 30 nights.");
        }

        return new DateRange(checkIn.Date, checkOut.Date);
    }

    /// <summary>
    /// Kiểm tra xem DateRange này có overlap với DateRange khác không
    /// VD: 
    /// - Range1: 1/1 -> 3/1
    /// - Range2: 2/1 -> 4/1
    /// → Overlap = true
    /// 
    /// Dùng để check availability:
    /// Nếu phòng đã được đặt trong range A, 
    /// thì không thể đặt trong range B nếu A và B overlap
    /// </summary>
    public bool OverlapsWith(DateRange other)
    {
        return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
    }

    /// <summary>
    /// Kiểm tra xem một ngày cụ thể có nằm trong DateRange không
    /// VD: Range: 1/1 -> 3/1
    ///     Date: 2/1 → true
    ///     Date: 3/1 → false (CheckOut không tính)
    /// </summary>
    public bool Contains(DateTime date)
    {
        return date.Date >= CheckIn.Date && date.Date < CheckOut.Date;
    }

    /// <summary>
    /// Override từ ValueObject base class
    /// Định nghĩa các giá trị dùng để so sánh 2 DateRange
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CheckIn;
        yield return CheckOut;
    }

    /// <summary>
    /// Override ToString() để dễ debug
    /// </summary>
    public override string ToString()
    {
        return $"{CheckIn:dd/MM/yyyy} - {CheckOut:dd/MM/yyyy} ({Nights} nights)";
    }
}