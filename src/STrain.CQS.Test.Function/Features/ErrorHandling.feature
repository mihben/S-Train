Feature: [ERH] - ErrorHandling

S-Train provides default error handling.

	Rule: [ERH/ERR] - Error response
		Used status codes:
		    - __400__: Validation or verification error.
			- __401__: Unathorized request.
			- __403__: Forbidden request.
			- __404__: Resource not found.
			- __500__: Internal servcer error.

	@issue-23
	@api
	Scenario Outline: [API][ERH/ERR-001] - Not found
		When Calling '<Endpoint>' endpoint
		Then Error response should be
			| Code | ContentType              | Type   | Title   | Status | Detail   | Instance   |
			| 404  | application/problem+json | <Type> | <Title> | 404    | <Detail> | <Instance> |

		Examples: 
			| Description        | Endpoint                | Type                       | Title               | Detail                                             | Instance                 |
			| Resource not found | api/error/not-found     | /errors/resource-not-found | Resource not found. | Resource '{resource}' was not found.               | /api/error/not-found     |
			| Endpoint not found | api/error/fake-endpoint | /errors/endpoint-not-found | Endpoint not found. | Endpoint '/api/error/fake-endpoint' was not found. | /api/error/fake-endpoint |

	@issue-23
	@api
	@unathorized
	Scenario: [API][ERH/ERR-002] - Unathorized
		When Calling '/api/error/unathorized' endpoint
		Then Error response should be
			| Code | ContentType              | Type                | Title                | Status | Detail                                                                   | Instance               |
			| 401  | application/problem+json | /errors/unathorized | Unathorized request. | 401    | Authentication is required for access '/api/error/unathorized' endpoint. | /api/error/unathorized |

	@issue-23
	@api
	@forbidden
	Scenario: [API][ERH/ERR-003] - Forbidden request
		When Calling '/api/error/forbidden' endpoint
		Then Error response should be
			| Code | ContentType              | Type              | Title      | Status | Detail                                                                      | Instance             |
			| 403  | application/problem+json | /errors/forbidden | Forbidden. | 403    | Specific permission is required for access '/api/error/forbidden' endpoint. | /api/error/forbidden |

	@issue-23
	@api
	Scenario: [API][ERH/ERR-005] - Internal server error
		When Calling '/api/error/internal-server-error' endpoint
		Then Error response should be
			| Code | ContentType              | Type                          | Title                  | Status | Detail                                               | Instance                         |
			| 500  | application/problem+json | /errors/internal-server-error | Internal server error. | 500    | Unexpected error happened. Please, call the support. | /api/error/internal-server-error |
				
	@issue-23
	@api
	Scenario: [API][ERH/ERR-006] - Validation error
		When Calling '/api/error/validation-error' endpoint
		Then Error response should be
			| Code | ContentType              | Type                    | Title            | Status | Detail                           | Instance                    | Errors.Property | Errors.Message             |
			| 400  | application/problem+json | /errors/invalid-request | Invalid request. | 400    | Invalid request. See the errors. | /api/error/validation-error | Value           | 'Value' must not be empty. |
				
	@issue-23
	@api
	Scenario: [API][ERH/ERR-007] - Verification error
		When Calling '/api/error/verification-error' endpoint
		Then Error response should be
			| Code | ContentType              | Type                              | Title               | Status | Detail                                                                    | Instance                       |
			| 400  | application/problem+json | /errors/custom-verification-error | Verification error. | 400    | Custom verification error. Can be used for business logic related errors. | /api/error/verification-error |