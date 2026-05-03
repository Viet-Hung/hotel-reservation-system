using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.Entities;
using HotelReservation.Domain.Reservations.Events;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Reservations.Entities;

/// <summary>
/// Unit tests cho Reservation aggregate root
/// 
/// Test naming: MethodName_Scenario_ExpectedBehavior
/// </summary>
public class ReservationTests
{
    // =========================
    // TEST HELPERS (Tái sử dụng data)
    // =========================

    /// <summary>
    /// Tạo valid DateRange cho tests
    /// </summary>
    private static DateRange CreateValidDateRange()
    {
        return DateRange.Create(
            DateTime.Today.AddDays(7),   // CheckIn sau 7 ngày
            DateTime.Today.AddDays(10)); // CheckOut sau 10 ngày
    }

    /// <summary>
    /// Tạo valid GuestInfo cho tests
    /// </summary>
    private static GuestInfo CreateValidGuestInfo()
    {
        return GuestInfo.Create(
            "John Doe",
            "john.doe@example.com",
            "+1 234 567 8900");
    }

    /// <summary>
    /// Tạo valid Money cho tests
    /// </summary>
    private static Money CreateValidTotalPrice()
    {
        return Money.Usd(300); // 100 USD/night * 3 nights
    }

    // =========================
    // CREATION TESTS
    // =========================

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var stay = CreateValidDateRange();
        var guest = CreateValidGuestInfo();
        var totalPrice = CreateValidTotalPrice();

        // Act
        var reservation = Reservation.Create(
            roomId,
            userId,
            stay,
            guest,
            totalPrice);

        // Assert
        reservation.Should().NotBeNull();
        reservation.ReservationId.Should().NotBeNull();
        reservation.RoomId.Should().Be(roomId);
        reservation.UserId.Should().Be(userId);
        reservation.Stay.Should().Be(stay);
        reservation.Guest.Should().Be(guest);
        reservation.TotalPrice.Should().Be(totalPrice);
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        reservation.ConfirmedAt.Should().BeNull();
        reservation.CancelledAt.Should().BeNull();
        reservation.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldRaiseReservationCreatedEvent()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var stay = CreateValidDateRange();
        var guest = CreateValidGuestInfo();
        var totalPrice = CreateValidTotalPrice();

        // Act
        var reservation = Reservation.Create(
            roomId,
            userId,
            stay,
            guest,
            totalPrice);

        // Assert
        reservation.DomainEvents.Should().HaveCount(1);
        var domainEvent = reservation.DomainEvents.First();
        domainEvent.Should().BeOfType<ReservationCreatedEvent>();

        var createdEvent = (ReservationCreatedEvent)domainEvent;
        createdEvent.ReservationId.Should().Be(reservation.ReservationId);
        createdEvent.GuestEmail.Should().Be(guest.Email);
        createdEvent.GuestName.Should().Be(guest.FullName);
        createdEvent.CheckIn.Should().Be(stay.CheckIn);
        createdEvent.CheckOut.Should().Be(stay.CheckOut);
    }

    [Fact]
    public void Create_WithEmptyRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var roomId = Guid.Empty;
        var userId = Guid.NewGuid();
        var stay = CreateValidDateRange();
        var guest = CreateValidGuestInfo();
        var totalPrice = CreateValidTotalPrice();

        // Act
        var act = () => Reservation.Create(roomId, userId, stay, guest, totalPrice);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Room ID is required.");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.Empty;
        var stay = CreateValidDateRange();
        var guest = CreateValidGuestInfo();
        var totalPrice = CreateValidTotalPrice();

        // Act
        var act = () => Reservation.Create(roomId, userId, stay, guest, totalPrice);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("User ID is required.");
    }

    [Fact]
    public void Create_WithZeroTotalPrice_ShouldThrowDomainException()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var stay = CreateValidDateRange();
        var guest = CreateValidGuestInfo();
        var totalPrice = Money.Zero("USD");

        // Act
        var act = () => Reservation.Create(roomId, userId, stay, guest, totalPrice);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Total price must be greater than zero.");
    }

    // =========================
    // CONFIRM TESTS
    // =========================

    [Fact]
    public void Confirm_FromPendingStatus_ShouldSucceed()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.ClearDomainEvents(); // Clear creation event

        // Act
        reservation.Confirm();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.Should().NotBeNull();
        reservation.ConfirmedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Confirm_ShouldRaiseReservationConfirmedEvent()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.ClearDomainEvents();

        // Act
        reservation.Confirm();

        // Assert
        reservation.DomainEvents.Should().HaveCount(1);
        var domainEvent = reservation.DomainEvents.First();
        domainEvent.Should().BeOfType<ReservationConfirmedEvent>();

        var confirmedEvent = (ReservationConfirmedEvent)domainEvent;
        confirmedEvent.ReservationId.Should().Be(reservation.ReservationId);
        confirmedEvent.GuestEmail.Should().Be(reservation.Guest.Email);
        confirmedEvent.CheckIn.Should().Be(reservation.Stay.CheckIn);
        confirmedEvent.CheckOut.Should().Be(reservation.Stay.CheckOut);
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_ShouldThrowDomainException()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm(); // Confirm lần 1

        // Act
        var act = () => reservation.Confirm(); // Confirm lần 2

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Reservation is already confirmed.");
    }

    [Fact]
    public void Confirm_WhenCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Cancel(); // Cancel trước

        // Act
        var act = () => reservation.Confirm(); // Cố confirm

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot confirm a cancelled reservation.");
    }

    // =========================
    // CANCEL TESTS
    // =========================

    [Fact]
    public void Cancel_FromPendingStatus_ShouldSucceed()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.ClearDomainEvents();

        // Act
        reservation.Cancel("Customer request");

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancelledAt.Should().NotBeNull();
        reservation.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        reservation.CancellationReason.Should().Be("Customer request");
    }

    [Fact]
    public void Cancel_FromConfirmedStatus_ShouldSucceed()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();
        reservation.ClearDomainEvents();

        // Act
        reservation.Cancel("Changed plans");

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancellationReason.Should().Be("Changed plans");
    }

    [Fact]
    public void Cancel_ShouldRaiseReservationCancelledEvent()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.ClearDomainEvents();

        // Act
        reservation.Cancel("Test cancellation");

        // Assert
        reservation.DomainEvents.Should().HaveCount(1);
        var domainEvent = reservation.DomainEvents.First();
        domainEvent.Should().BeOfType<ReservationCancelledEvent>();

        var cancelledEvent = (ReservationCancelledEvent)domainEvent;
        cancelledEvent.ReservationId.Should().Be(reservation.ReservationId);
        cancelledEvent.GuestEmail.Should().Be(reservation.Guest.Email);
        cancelledEvent.CancellationReason.Should().Be("Test cancellation");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Cancel();

        // Act
        var act = () => reservation.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Reservation is already cancelled.");
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrowDomainException()
    {
        // Arrange
        // Tạo reservation với checkout trong quá khứ
        var pastStay = DateRange.Create(
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(-7));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            pastStay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();
        reservation.Complete();

        // Act
        var act = () => reservation.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel a completed reservation.");
    }

    [Fact]
    public void Cancel_AfterCheckInDate_ShouldThrowDomainException()
    {
        // Arrange
        // Tạo reservation với checkin hôm nay
        var stay = DateRange.Create(
            DateTime.Today,
            DateTime.Today.AddDays(3));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            stay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        // Act
        var act = () => reservation.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel reservation after check-in date.");
    }

    [Fact]
    public void Cancel_WithoutReason_ShouldSucceed()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        // Act
        reservation.Cancel(); // Không truyền reason

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancellationReason.Should().BeNull();
    }

    // =========================
    // COMPLETE TESTS
    // =========================

    [Fact]
    public void Complete_FromConfirmedStatus_ShouldSucceed()
    {
        // Arrange
        // Tạo reservation đã checkout
        var pastStay = DateRange.Create(
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(-7));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            pastStay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();

        // Act
        reservation.Complete();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Completed);
    }

    [Fact]
    public void Complete_FromPendingStatus_ShouldThrowDomainException()
    {
        // Arrange
        var pastStay = DateRange.Create(
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(-7));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            pastStay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        // Act
        var act = () => reservation.Complete();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Only confirmed reservations can be completed.");
    }

    [Fact]
    public void Complete_BeforeCheckOutDate_ShouldThrowDomainException()
    {
        // Arrange
        var futureStay = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(5));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            futureStay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();

        // Act
        var act = () => reservation.Complete();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot complete reservation before check-out date.");
    }

    // =========================
    // DOMAIN EVENTS MANAGEMENT TESTS
    // =========================

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.DomainEvents.Should().HaveCount(1); // ReservationCreatedEvent

        // Act
        reservation.ClearDomainEvents();

        // Assert
        reservation.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MultipleActions_ShouldAccumulateEvents()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        // Act
        reservation.Confirm();

        // Assert - Có 2 events: Created + Confirmed
        reservation.DomainEvents.Should().HaveCount(2);
        reservation.DomainEvents.First().Should().BeOfType<ReservationCreatedEvent>();
        reservation.DomainEvents.Last().Should().BeOfType<ReservationConfirmedEvent>();
    }

    // =========================
    // STATE TRANSITION TESTS
    // =========================

    [Fact]
    public void StateTransition_Pending_To_Confirmed_ShouldBeValid()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Status.Should().Be(ReservationStatus.Pending);

        // Act
        reservation.Confirm();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public void StateTransition_Pending_To_Cancelled_ShouldBeValid()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Status.Should().Be(ReservationStatus.Pending);

        // Act
        reservation.Cancel();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void StateTransition_Confirmed_To_Cancelled_ShouldBeValid()
    {
        // Arrange
        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreateValidDateRange(),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);

        // Act
        reservation.Cancel();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void StateTransition_Confirmed_To_Completed_ShouldBeValid()
    {
        // Arrange
        var pastStay = DateRange.Create(
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(-7));

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            pastStay,
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        reservation.Confirm();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);

        // Act
        reservation.Complete();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Completed);
    }

    // =========================
    // EDGE CASES
    // =========================

    [Fact]
    public void Create_WithSameRoomAndUser_ShouldSucceed()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act - Tạo 2 reservations cùng room và user (khác thời gian)
        var reservation1 = Reservation.Create(
            roomId,
            userId,
            DateRange.Create(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3)),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        var reservation2 = Reservation.Create(
            roomId,
            userId,
            DateRange.Create(DateTime.Today.AddDays(5), DateTime.Today.AddDays(7)),
            CreateValidGuestInfo(),
            CreateValidTotalPrice());

        // Assert - 2 reservations khác nhau
        reservation1.ReservationId.Should().NotBe(reservation2.ReservationId);
        reservation1.RoomId.Should().Be(reservation2.RoomId);
        reservation1.UserId.Should().Be(reservation2.UserId);
    }
}