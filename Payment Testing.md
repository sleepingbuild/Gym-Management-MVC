# Payment Testing Plan

## 1. Unit Tests

### 1.1 PaymentService Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-001 | Create payment with valid data | Success |
| TC-002 | Create payment with invalid membership | KeyNotFoundException |
| TC-003 | Update payment status to Success | Status updated |
| TC-004 | Update payment with invalid id | KeyNotFoundException |
| TC-005 | Get payment history with date filter | Returns filtered list |
| TC-006 | Get payment history with no filter | Returns all payments |
| TC-007 | Get payment history by status | Returns filtered by status |
| TC-008 | Search payments by transaction id | Returns matching payment |
| TC-009 | Search payments by payment info | Returns matching payment |
| TC-010 | Get payment statistics | Returns correct statistics |
| TC-011 | Process VNPay success response | Payment updated to Success |
| TC-012 | Process VNPay failure response | Payment updated to Failed |
| TC-013 | Create VNPay payment URL | Returns valid URL |

### 1.2 PaymentController Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-014 | GET Index with authenticated user | Returns view |
| TC-015 | GET Create with valid membership | Returns view |
| TC-016 | GET Create with invalid membership | Redirect with error |
| TC-017 | GET History | Returns view |
| TC-018 | GET Statistics | Returns view |
| TC-019 | GET Details with valid id | Returns view |
| TC-020 | GET Details with invalid id | NotFound |

## 2. Integration Tests

### 2.1 VNPay Flow Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-021 | Complete payment flow | Payment Success, Membership Active |
| TC-022 | Failed payment flow | Payment Failed |
| TC-023 | VNPay callback with valid data | Payment updated |
| TC-024 | VNPay callback with invalid signature | Rejected |

## 3. Manual Test Cases

### 3.1 User Flow Tests
| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| TC-025 | User creates payment | Redirect to VNPay |
| TC-026 | User completes payment | Payment marked Success |
| TC-027 | User views payment history | Show all payments |
| TC-028 | User views statistics | Show statistics |
| TC-029 | User exports CSV | CSV file downloaded |

## 4. Edge Cases

### 4.1 Boundary Tests
| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| TC-030 | Payment amount = 0 | Validation error |
| TC-031 | Payment with null membership | Handled gracefully |
| TC-032 | Duplicate transaction id | Handled gracefully |
| TC-033 | Search with empty keyword | Show all payments |
| TC-034 | Filter with date range | Show filtered results |
| TC-035 | VNPay timeout | Error handled gracefully |

## 5. Run Tests

### Run Unit Tests
```bash
dotnet test --filter "FullyQualifiedName~PaymentServiceTests"
dotnet test --filter "FullyQualifiedName~PaymentControllerTests"