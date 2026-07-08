# Booking Testing Plan

## 1. Unit Tests

### 1.1 BookingService Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-001 | Create booking with valid data | Success |
| TC-002 | Create booking with invalid trainer | Exception |
| TC-003 | Create booking when slot is booked | Exception |
| TC-004 | Cancel booking with valid id | Success |
| TC-005 | Cancel booking with invalid id | Returns false |
| TC-006 | Confirm booking | Status changed to Confirmed |
| TC-007 | Complete booking | Status changed to Completed |
| TC-008 | Get booking history by date range | Returns filtered list |
| TC-009 | Search bookings by keyword | Returns matching results |

### 1.2 BookingRepository Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-010 | Add booking | Success |
| TC-011 | Get booking by id | Returns booking |
| TC-012 | Get bookings by user id | Returns user's bookings |
| TC-013 | Get bookings by trainer id | Returns trainer's bookings |
| TC-014 | Get bookings by date | Returns bookings on date |
| TC-015 | Check slot availability | Returns true/false |

## 2. Integration Tests

### 2.1 BookingController Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-016 | GET Index with authenticated user | Returns view with bookings |
| TC-017 | GET Index with unauthenticated user | Redirect to Login |
| TC-018 | GET Create with authenticated user | Returns view with trainers |
| TC-019 | POST Create with valid data | Success and redirect |
| TC-020 | POST Create with invalid data | Show error message |
| TC-021 | POST Cancel with valid id | Success and redirect |
| TC-022 | GET History with filter | Returns filtered results |
| TC-023 | GET Statistics | Returns statistics view |

## 3. Manual Test Cases

### 3.1 User Flow Tests
| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| TC-024 | User creates a booking | Booking created, status Pending |
| TC-025 | User views booking list | Show all bookings |
| TC-026 | User cancels booking | Status changed to Cancelled |
| TC-027 | User views booking history | Show history with filters |
| TC-028 | User views statistics | Show booking statistics |

## 4. Edge Cases

### 4.1 Boundary Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-029 | Create booking for past date | Error message |
| TC-030 | Create booking for same slot | Error message |
| TC-031 | Cancel already cancelled booking | Error message |
| TC-032 | Search with empty keyword | Show all results |
| TC-033 | Filter with date range | Show filtered results |

## 5. Run Tests

### Run Unit Tests
```bash
dotnet test --filter "FullyQualifiedName~BookingServiceTests"
dotnet test --filter "FullyQualifiedName~BookingRepositoryTests"